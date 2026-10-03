using FlowerShop.Dtos;
using Microsoft.EntityFrameworkCore;
using FlowerShop.Data;
using FlowerShop.Dtos;
using FlowerShop.Models;

namespace FlowerShop.Services 
{
    public class FlowerService : IFlowerService
    {
        private readonly ShopDbContext _db;

        public FlowerService(ShopDbContext db)
        {
            _db = db;
        }
        public async Task<List<FlowerDto>> GetAllAsync(int? categoryId, string? search, string sort)
        {
            var query = _db.Flowers.Include(f => f.Category).Include(f => f.Supplier).AsQueryable();
            if (categoryId.HasValue)
            {
                query = query.Where(f => f.CategoryId == categoryId.Value);
            }
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(f => f.Name.Contains(search));
            }
            if (sort == "price")
            {
                query = query.OrderBy(f => f.Price);
            }
            else
            {
                query = query.OrderBy(f => f.Name);
            }
            var result = await query
                .Select(f => new FlowerDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    Price = f.Price,
                    CategoryName = f.Category.Name,
                    SupplierName = f.Supplier.Name
                })
                .ToListAsync();
            return result;
        }
        public async Task<FlowerDto?> GetByIdAsync(int id)
        {
            var flower = await _db.Flowers.Include(f => f.Category).Include(f => f.Supplier).FirstOrDefaultAsync(f => f.Id == id);
            if(flower == null)
            {
                return null;
            }
            else
            {
                return new FlowerDto
                {
                    Id = flower.Id,
                    Name = flower.Name,
                    Price = flower.Price,
                    CategoryName = flower.Category.Name,
                    SupplierName = flower.Supplier.Name
                };
            }
        }
        public async Task<FlowerDto> CreateAsync(FlowerRequest request)
        {
            Flower flower = new Flower
            {
                Name = request.Name,
                Price = request.Price,
                CategoryId = request.CategoryId,
                SupplierId = request.SupplierId
            };
            _db.Flowers.Add(flower);
            await _db.SaveChangesAsync();
            var category = await _db.Categories.FindAsync(flower.CategoryId);
            var supplier = await _db.Suppliers.FindAsync(flower.SupplierId);
            return new FlowerDto
            {
                Id = flower.Id,
                Name = flower.Name,
                Price = flower.Price,
                CategoryName = category.Name,
                SupplierName = supplier.Name
            };
        }
        public async Task<bool> UpdateAsync(int id, FlowerRequest request)
        {
            var flower = await _db.Flowers.FindAsync(id);
            if (flower == null)
            {
                return false;
            }
            flower.Name = request.Name;
            flower.Price = request.Price;
            flower.CategoryId = request.CategoryId;
            flower.SupplierId = request.SupplierId;
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var flower = await _db.Flowers.FindAsync(id);
            if (flower == null)
            {
                return false;
            }
            _db.Flowers.Remove(flower);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}