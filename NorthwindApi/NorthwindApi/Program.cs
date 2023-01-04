using Microsoft.EntityFrameworkCore;
using NorthwindApi.Models;
using NorthwindAPI.Services;

namespace NorthwindApi {
    public class Program {
        public static void Main( string[] args ) {
            var builder = WebApplication.CreateBuilder( args );
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            builder.Services.AddDbContext<NorthwindContext>( opt => opt.UseSqlServer( builder.Configuration[ "default" ] ) );

            builder.Services.AddControllersWithViews()
                .AddNewtonsoftJson( options =>
                    options.SerializerSettings.ReferenceLoopHandling =
                    Newtonsoft.Json.ReferenceLoopHandling.Ignore );

            builder.Services.AddScoped<IService<Supplier>, SupplierService>();
            builder.Services.AddScoped<IService<Product>, ProductService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if ( app.Environment.IsDevelopment() ) {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}