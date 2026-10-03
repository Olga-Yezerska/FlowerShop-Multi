using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlowerShop.Models;
using FlowerShop.Data;
using FlowerShop.Services;
using FlowerShop.Dtos;

namespace FlowerShop.Controllers.Api
{
    public class FlowerApiController
    {
        [ApiController]
        [Route("api/[controller]")]
        public class FlowersController : ControllerBase
        {
            private readonly IFlowerService _flowerService;

            public FlowersController(IFlowerService flowerService)
            {
                _flowerService = flowerService;
            }

            // методи GET/POST/PUT/DELETE 
            [HttpGet]
            public async Task<IActionResult> GetAll(int? categoryId, string? search, string? sort)
            {
                var flowers = await _flowerService.GetAllAsync(categoryId, search, sort ?? "name");
                return Ok(flowers);
            }

            [HttpGet("{id}")]
            public async Task<IActionResult> GetById(int id)
            {
                var flower = await _flowerService.GetByIdAsync(id);
                if (flower == null)
                {
                    return NotFound();
                }
                return Ok(flower);
            }

            [HttpPost]
            public async Task<IActionResult> Create([FromBody] FlowerRequest request)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var createdFlower = await _flowerService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = createdFlower.Id }, createdFlower);
            }

            [HttpPut("{id}")]
            public async Task<IActionResult> Update(int id, [FromBody] FlowerRequest request)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var updated = await _flowerService.UpdateAsync(id, request);
                if (!updated)
                {
                    return NotFound();
                }
                return NoContent();
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> Delete(int id)
            {
                var deleted = await _flowerService.DeleteAsync(id);
                if (!deleted)
                {
                    return NotFound();
                }
                return NoContent();
            }
        }
    }
}
