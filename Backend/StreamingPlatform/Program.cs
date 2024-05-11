using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StreamingPlatform
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string? databaseConnectionString = builder.Configuration.GetConnectionString("StreamingServiceDB");

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddControllers().AddJsonOptions(
             options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            builder.Services.AddDbContext<StreamingDbContext>(options => options.UseSqlServer(databaseConnectionString));
            builder.Services.AddScoped<IPlanService, PlanService>();
            builder.Services.AddResponseCaching();
            builder.Services.AddOutputCache();
            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
            builder.Services.AddControllers().AddJsonOptions(
                options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            builder.Services.AddDbContext<StreamingDbContext>(options => options.UseSqlServer(databaseConnectionString))
                .AddDbContext<AuthDbContext>(options => options.UseSqlServer(databaseConnectionString));

            // Identity
            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();
            AddRateLimiting(builder);

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

            app.UseHttpsRedirection();
            app.UseResponseCaching();
            app.UseOutputCache();

            app.MapControllers();
            app.Run();
        }

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