using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlowerShop.Models;
using FlowerShop.Data;
using FlowerShop.Services;
using FlowerShop.Dtos;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace FlowerShop.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User,Admin")]
    public class FlowersController : ControllerBase
    {
        private readonly IFlowerService _flowerService;
        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private bool IsAdmin => User.IsInRole("Admin");

        public FlowersController(IFlowerService flowerService)
        {
            _flowerService = flowerService;
        }

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
            var createdFlower = await _flowerService.CreateAsync(request, CurrentUserId);
            return CreatedAtAction(nameof(GetById), new { id = createdFlower.Id }, createdFlower);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FlowerRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _flowerService.UpdateAsync(id, request, CurrentUserId, IsAdmin);
            if (result == OperationResult.NotFound)
            {
                return NotFound();
            }
            if (result == OperationResult.Forbidden)
            {
                return Forbid();
            }
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _flowerService.DeleteAsync(id, CurrentUserId, IsAdmin);
            if (result == OperationResult.NotFound)
            {
                return NotFound();
            }
            if (result == OperationResult.Forbidden)
            {
                return Forbid();
            }
            return NoContent();
        }
    }
}