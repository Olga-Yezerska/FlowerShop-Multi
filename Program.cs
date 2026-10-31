using System.Text;
using FlowerShop.Data;
using FlowerShop.Models;
using FlowerShop.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace FlowerShop
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // База даних: перемикання через конфігурацію
            var provider = builder.Configuration["DatabaseProvider"];
            builder.Services.AddDbContext<ShopDbContext>(options =>
            {
                if (provider == "Sqlite")
                    options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
                else if (provider == "SqlServer")
                    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
            });
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IFlowerService, FlowerService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            // JWT: ключ береться з User Secrets
            builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            // Policy
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            });

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Default", policy =>
                    policy.WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
            });

            // Swagger із підтримкою Bearer
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Name = "Authorization"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            var app = builder.Build();

            // Міграції та початкові дані
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
                await db.Database.MigrateAsync();

                if (!await db.Categories.AnyAsync())
                {
                    db.Categories.AddRange(
                        new Category { Name = "Roses" },
                        new Category { Name = "Tulips" },
                        new Category { Name = "Orchids" },
                        new Category { Name = "Sunflowers" },
                        new Category { Name = "Bouquets" });
                }

                if (!await db.Suppliers.AnyAsync())
                {
                    db.Suppliers.AddRange(
                        new Supplier { Name = "GreenLeaf Farms" },
                        new Supplier { Name = "Bloom Wholesale" },
                        new Supplier { Name = "Sunny Fields Co" },
                        new Supplier { Name = "Petal & Stem Supply" });
                }

                await db.SaveChangesAsync();
            }

            // Помилки: клієнт не бачить технічних деталей
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{\"message\":\"Internal server error\"}");
                });
            });

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseCors("Default");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Flower}/{action=Index}/{id?}");

            app.Run();
        }
    }
}