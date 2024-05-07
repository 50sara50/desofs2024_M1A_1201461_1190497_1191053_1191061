using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StreamingPlatform.Dao;
using StreamingPlatform.Models;
using System.Data;
using System.Text;

namespace StreamingPlatform
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var databaseConnectionString = builder.Configuration.GetConnectionString("StreamingServiceDB");

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddDbContext<StreamingDbContext>(options => options.UseInMemoryDatabase("StreamingServiceDB"));
            builder.Services.AddDbContext<AuthDbContext>(options => options.UseInMemoryDatabase("StreamingServiceDB"));
            //builder.Services.AddDbContext<StreamingDbContext>(options => options.UseSqlServer(databaseConnectionString));

            // Identity
            builder.Services.AddIdentity<User, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<StreamingDbContext>()
                .AddDefaultTokenProviders();

            // Authentication
            builder.Services.AddAuthentication(options => 
            {
                options.DefaultAuthenticateScheme =  JwtBearerDefaults.AuthenticationScheme;
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
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
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

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}