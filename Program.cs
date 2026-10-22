using Microsoft.EntityFrameworkCore;
using FlowerShop.Models;
using FlowerShop.Data;
using FlowerShop.Services;
using FlowerShop.Dtos;
namespace FlowerShop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var provider = builder.Configuration["DatabaseProvider"];

            builder.Services.AddDbContext<ShopDbContext>(options =>
            {
                if (provider == "Sqlite")
                {
                    options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
                }
                else if (provider == "SqlServer")
                {
                    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
                }
            });

            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<IFlowerService, FlowerService>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Flower}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
