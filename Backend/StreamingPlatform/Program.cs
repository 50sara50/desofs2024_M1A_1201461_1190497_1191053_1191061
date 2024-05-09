using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using StreamingPlatform.Dao;
using StreamingPlatform.Dao.Interfaces;
using StreamingPlatform.Dao.Repositories;
using StreamingPlatform.Services;
using StreamingPlatform.Services.Interfaces;

namespace StreamingPlatform
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string? databaseConnectionString;
            if (builder.Environment.IsDevelopment())
            {
                databaseConnectionString = builder.Configuration.GetConnectionString("StreamingServiceDB_DEV");
            }
            else
            {
                databaseConnectionString = builder.Configuration.GetConnectionString("StreamingServiceDB");
            }

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddControllers().AddJsonOptions(
             options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            builder.Services.AddDbContext<StreamingDbContext>(options => options.UseSqlServer(databaseConnectionString));
            builder.Services.AddScoped<IPlanService, PlanService>();
            builder.Services.AddResponseCaching();
            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

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
            app.UseResponseCaching();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}