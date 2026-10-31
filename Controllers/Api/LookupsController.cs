using FlowerShop.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Controllers.Api
{
    [ApiController]
    [Route("api/lookups")]
    [Authorize(Roles = "User,Admin")]
    public class LookupsController : ControllerBase
    {
        private readonly ShopDbContext _db;

        public LookupsController(ShopDbContext db)
        {
            _db = db;
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var items = await _db.Categories.OrderBy(c => c.Name)
                .Select(c => new { c.Id, c.Name }).ToListAsync();
            return Ok(items);
        }

        [HttpGet("suppliers")]
        public async Task<IActionResult> GetSuppliers()
        {
            var items = await _db.Suppliers.OrderBy(s => s.Name)
                .Select(s => new { s.Id, s.Name }).ToListAsync();
            return Ok(items);
        }
    }
}