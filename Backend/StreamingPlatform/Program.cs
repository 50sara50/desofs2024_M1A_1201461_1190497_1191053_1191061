using Microsoft.EntityFrameworkCore;
using StreamingPlatform.Dao;

namespace StreamingPlatform
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var databaseConnectionString = builder.Configuration.GetConnectionString("MariaDB");
            var serverVersion = new MariaDbServerVersion(new Version(11, 2, 2));
            
            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddDbContext<StreamingDbContext>(options => options.UseMySql(databaseConnectionString, serverVersion));
            
            builder.Services.AddScoped<IPlaylistService, PlaylistService>();
            
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

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}