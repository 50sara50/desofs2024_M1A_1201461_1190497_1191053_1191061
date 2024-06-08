using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using StreamingPlatform.Configurations.Mapper;
using StreamingPlatform.Configurations.Models;
using StreamingPlatform.Controllers.Extensions;
using StreamingPlatform.Controllers.ResponseMapper;
using StreamingPlatform.Controllers.Responses;
using StreamingPlatform.Dao;
using StreamingPlatform.Dao.Interfaces;
using StreamingPlatform.Dao.Repositories;
using StreamingPlatform.Models;
using StreamingPlatform.Services;
using StreamingPlatform.Services.Interfaces;
using StreamingPlatform.Utils;
using ExceptionHandlerMiddleware = StreamingPlatform.Controllers.Middleware.ExceptionHandlerMiddleware;

namespace StreamingPlatform
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // LOGGING
            builder.Logging.ClearProviders().AddConsole(options =>
            {
                options.IncludeScopes = builder.Configuration.GetValue<bool>("Logging:Console:IncludeScopes");
                options.TimestampFormat = builder.Configuration.GetValue<string>("Logging:Console:FormatterOptions:TimestampFormat");
                options.UseUtcTimestamp = builder.Configuration.GetValue<bool>("Logging:Console:FormatterOptions:UseUtcTimestamp");
            });
            AddCors(builder);

            var databaseConnectionString = builder.Configuration.GetConnectionString("StreamingServiceDB");
            builder.Services.AddControllers().AddJsonOptions(
             options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            // Add services to the container.
            builder.Services.AddScoped<IPlanService, PlanService>();
            builder.Services.AddScoped<IPlaylistService, PlaylistService>();
            builder.Services.AddScoped<ISongService, SongService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            AddOutPutCaching(builder);
            AddAuthorizationPolicies(builder);
            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

            if (builder.Environment.IsDevelopment())
            {
                builder.Services
                    .AddDbContext<StreamingDbContext>(options => options.UseInMemoryDatabase("DB"));
            }
            else
            {
                builder.Services
                    .AddDbContext<StreamingDbContext>(options => options.UseSqlServer(databaseConnectionString));
            }

            // Identity
            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<StreamingDbContext>()
                .AddDefaultTokenProviders();
            AddRateLimiting(builder);
            AddOutPutCaching(builder);
            AddHst(builder);

            // Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                    .AddJwtBearer(options =>
                    {
                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                if (context.Request.Cookies.ContainsKey("__Host-userBearerToken"))
                                {
                                    context.Token = context.Request.Cookies["__Host-userBearerToken"];
                                }

                                return Task.CompletedTask;
                            },
                        };
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

            // Add CORS middleware
            app.UseCors("CorsPolicy");

            // ASVS.7.4.1
            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseInputSanitization();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();
            app.UseOutputCache();
            app.UseHttpsRedirection();
            app.UseHsts();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(builder.Environment.WebRootPath, "Songs")),
                RequestPath = "/song",
                ContentTypeProvider = new FileExtensionContentTypeProvider()
                {
                    Mappings = { [".mp3"] = "audio/mpeg", [".wav"] = "audio/wave", [".m4a"] = "audio/mp4", [".txt"] = "text/plain" },
                },
                ServeUnknownFileTypes = false,
            }).UseAuthentication().UseAuthorization();

            app.Use(async (context, next) =>
            {
                context.Response.OnStarting(
                    _ =>
                    {
                        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
                        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
                        return Task.CompletedTask;
                    }, context);
                await next();
            });

            app.MapControllers();
            app.Run();
        }

        private static void AddCors(WebApplicationBuilder builder)
        {
            //Add Cors
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    "CorsPolicy",
                    builder =>
                    {
                        builder.
                                WithOrigins("http://localhost:4200", "https://localhost:4200").
                                AllowAnyMethod().
                                AllowAnyHeader().
                                AllowCredentials();
                    });

            });
        }

        private static void AddHst(WebApplicationBuilder builder)
        {
            builder.Services.AddHsts(options =>
            {
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(182);
            });
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

        private static void AddAuthorizationPolicies(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("DownloadPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Admin", "Subscriber", "Artist");
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