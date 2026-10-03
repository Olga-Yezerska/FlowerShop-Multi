using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlowerShop.Models;
using FlowerShop.Data;
namespace FlowerShop.Controllers
{
    public class FlowerController : Controller
    {
        private readonly ShopDbContext _db;
        public FlowerController(ShopDbContext db)
        {
            _db = db;
        }
        // GET
        public async Task<IActionResult> Index(int? categoryId, string sort)
        {
            var query = _db.Flowers
                .Include(f => f.Category)
                .Include(f => f.Supplier)
                .AsQueryable();
            if (categoryId.HasValue)
            {
                query = query.Where(f => f.CategoryId == categoryId);
            }

            if (sort == "price")
            {
                query = query.OrderBy(f => f.Price);
            }
            else
            {
                query = query.OrderBy(f => f.Name);
            }

            var flowers = await query.ToListAsync();

            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View(flowers);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _db.Categories.ToListAsync();
            ViewBag.Suppliers = await _db.Suppliers.ToListAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Flower flower)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _db.Categories.ToListAsync();
                ViewBag.Suppliers = await _db.Suppliers.ToListAsync();
                return View(flower);
            }

            _db.Flowers.Add(flower);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var flower = await _db.Flowers.FindAsync(id);
            if (flower == null)
            {
                return NotFound();
            }
            ViewBag.Categories = await _db.Categories.ToListAsync();
            ViewBag.Suppliers = await _db.Suppliers.ToListAsync();
            return View(flower);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Flower flower)
        {
            var existingFlower = await _db.Flowers.FindAsync(id);
            if (existingFlower == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _db.Categories.ToListAsync();
                ViewBag.Suppliers = await _db.Suppliers.ToListAsync();
                return View(flower);
            }

            existingFlower.Name = flower.Name;
            existingFlower.Price = flower.Price;
            existingFlower.Category = await _db.Categories.FindAsync(flower.CategoryId);
            existingFlower.Supplier = await _db.Suppliers.FindAsync(flower.SupplierId);

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var flower = await _db.Flowers.FindAsync(id);
            if (flower == null)
            {
                return NotFound();
            }
            _db.Flowers.Remove(flower);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Stats()
        {
            var flowers = await _db.Flowers.Include(f => f.Category).ToListAsync();
            var stats = flowers
                .GroupBy(f => f.Category.Name)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    AveragePrice = g.Average(f => f.Price)
                })
                .ToList();
            return View(stats);
        }
    }
}
