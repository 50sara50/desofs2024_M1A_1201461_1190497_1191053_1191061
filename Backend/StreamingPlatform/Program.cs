using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using StreamingPlatform.Configurations.Mapper;
using StreamingPlatform.Configurations.Models;
using StreamingPlatform.Controllers.ResponseMapper;
using StreamingPlatform.Controllers.Responses;
using StreamingPlatform.Dao;
using StreamingPlatform.Dao.Interfaces;
using StreamingPlatform.Dao.Repositories;
using StreamingPlatform.Models;
using StreamingPlatform.Services;
using StreamingPlatform.Services.Interfaces;
using StreamingPlatform.Utils;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StreamingPlatform
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var databaseConnectionString = builder.Configuration.GetConnectionString("StreamingServiceDB");

            builder.Services.AddControllers().AddJsonOptions(
             options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            if (builder.Environment.IsDevelopment())
            {
                builder.Services
                    .AddDbContext<StreamingDbContext>(options => options.UseSqlServer(databaseConnectionString))
                    .AddDbContext<AuthDbContext>(options => options.UseSqlServer(databaseConnectionString));
            }
            else
            {
                builder.Services
                    .AddDbContext<StreamingDbContext>(options => options.UseSqlServer(databaseConnectionString))
                    .AddDbContext<AuthDbContext>(options => options.UseSqlServer(databaseConnectionString));
            }

            // Identity
            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();
            AddRateLimiting(builder);
            AddOutPutCaching(builder);

            // Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                    .AddJwtBearer(options =>
                    {
                        options.SaveToken = true;
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidIssuer = builder.Configuration["Jwt:Issuer"],
                            ValidAudience = builder.Configuration["Jwt:Issuer"],
                            IssuerSigningKey =
#pragma warning disable CS8604
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
#pragma warning restore CS8604
                        };
                    });

            builder.Services.AddScoped<IAuthService, AuthService>();
            // Add services to the container.
            builder.Services.AddScoped<IPlanService, PlanService>();
            builder.Services.AddScoped<ISongService, SongService>();
            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
            string encryptionKey = builder.Configuration.GetValue<string>("Keys:SecureDataKey") ?? throw new InvalidOperationException("SecureDataKey is not set in the configuration file.");
            SecureDataEncryptionHelper.SetEncryptionKey(encryptionKey);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();
            app.UseOutputCache();
            app.UseHttpsRedirection();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(builder.Environment.WebRootPath, "Songs")),
                RequestPath = "/Songs",
            });

            app.MapControllers();
            app.Run();
        }

        /// <summary>
        /// Adds rate limiting functionality to the application based on user identity or IP address.
        /// </summary>
        /// <param name="builder">The WebApplicationBuilder instance used to configure the application services.</param>

        private static void AddRateLimiting(WebApplicationBuilder builder)
        {
            IConfiguration configuration = builder.Configuration;
            FixedWindowRateLimiterConfig fixedWindowRateLimiterConfig = RateLimiterConfigMapper.MapToFixedWindowRateLimiterConfig(configuration);
            TokenBucketRateLimiterConfig tokenBucketRateLimiterConfig = RateLimiterConfigMapper.MapToTokenBucketRateLimiterConfig(configuration);

            builder.Services.AddRateLimiter(rateLimiter =>
            {
                rateLimiter.AddPolicy("fixed-by-user-id-or-ip", context =>
                {
                    if (context.User.Identity?.IsAuthenticated == true)
                    {
                        return RateLimitPartition.GetFixedWindowLimiter(context.User.Identity.Name, _ => new FixedWindowRateLimiterOptions
                        {
                            Window = TimeSpan.FromSeconds(fixedWindowRateLimiterConfig.Window),
                            PermitLimit = fixedWindowRateLimiterConfig.PermitLimit,
                        });
                    }

                    string? clientIpAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

                    if (string.IsNullOrEmpty(clientIpAddress))
                    {
                        clientIpAddress = context.Connection.RemoteIpAddress?.ToString();
                    }
                    else
                    {
                        clientIpAddress = clientIpAddress.Split(',').FirstOrDefault()?.Trim();
                    }

                    return RateLimitPartition.GetTokenBucketLimiter(clientIpAddress, _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = tokenBucketRateLimiterConfig.TokenLimit,
                        AutoReplenishment = tokenBucketRateLimiterConfig.AutoReplenishment,
                        ReplenishmentPeriod = TimeSpan.FromSeconds(tokenBucketRateLimiterConfig.ReplenishmentPeriod),
                        TokensPerPeriod = tokenBucketRateLimiterConfig.TokensPerPeriod,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    });
                });
                DefineOnRejectResponse(rateLimiter, tokenBucketRateLimiterConfig.ReplenishmentPeriod);
            });
        }

        private static void AddOutPutCaching(WebApplicationBuilder builder)
        {
            builder.Services.AddOutputCache(options =>
            {
                options.AddPolicy(
                    "evict",
                    builder =>
                {
                    builder.With(c => c.HttpContext.Request.Path.ToString().Contains("api/plan"))
                    .Tag("tag-plan");
                });
            });
        }

        /// <summary>
        /// Configures the response sent when a request is rejected due to exceeding rate limits.
        /// </summary>
        /// <param name="ratelimiter">The RateLimiterOptions instance for configuring rejection behavior.</param>
        /// <param name="retryAfter">The retry-after period in seconds to include in the response.</param>
        private static void DefineOnRejectResponse(RateLimiterOptions ratelimiter, double retryAfter)
        {
            ratelimiter.OnRejected = async (context, token) =>
            {
                HttpContext httpContext = context.HttpContext;
                httpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                httpContext.Response.Headers.Append("Retry-After", retryAfter.ToString());
                httpContext.Response.ContentType = "application/json";
                httpContext.RequestServices.GetService<ILoggerFactory>()?.CreateLogger("Microsoft.AspNetCore.RateLimitingMiddleware")
                .LogWarning("OnRejected: {GetUserEndPoint}", httpContext.Request.Path);
                ErrorResponseObject errorResponseObject = MapResponse.TooManyRequests();

                try
                {
                    await httpContext.Response.WriteAsJsonAsync(errorResponseObject, token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error writing JSON response: {ex.Message}");
                }
            };
        }
    }
}