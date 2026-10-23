using FlowerShop.Data;
using FlowerShop.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Controllers.Api
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : ControllerBase
    {
        private readonly ShopDbContext _db;

        public AdminController(ShopDbContext db)
        {
            _db = db;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _db.Users
                .Select(u => new UserDto { Id = u.Id, Username = u.Username, Role = u.Role })
                .ToListAsync();
            return Ok(users);
        }
    }
}