using Microsoft.EntityFrameworkCore;
using FlowerShop.Models;
namespace FlowerShop.Data
{
    public class ShopDbContext : DbContext
    {
        public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options) { }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<Flower> Flowers => Set<Flower>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Flower>()
                .Property(f => f.Price)
                .HasPrecision(18, 2);
        }
    }
}