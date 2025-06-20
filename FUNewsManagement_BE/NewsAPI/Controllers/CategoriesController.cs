using BusinessLogicLayer;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Security.Claims;

namespace NewsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Staff, Admin")]
    public class CategoriesController : ODataController
    {
        private readonly CategoryService _categoryService;

        public CategoriesController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        //private bool IsStaff()
        //{
        //    var role = User.FindFirst(ClaimTypes.Role)?.Value;
        //    return role == "Staff";
        //}

        //private short GetCurrentUserId()
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    return short.TryParse(userId, out var id) ? id : throw new UnauthorizedAccessException("User ID not found.");
        //}

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(short id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            return category is null ? NotFound() : Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "Staff")] // Chỉ Staff được tạo
        public async Task<IActionResult> Post([FromBody] Category category)
        {
            //if (!IsStaff()) return Forbid();

            await _categoryService.AddAsync(category);
            return CreatedAtAction(nameof(GetById), new { id = category.CategoryId }, category);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Staff")] // Chỉ Staff được sửa
        public async Task<IActionResult> Put(short id, [FromBody] Category category)
        {
            //if (!IsStaff()) return Forbid();

            var success = await _categoryService.UpdateAsync(id, category);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Staff")] // Chỉ Staff được xoá
        public async Task<IActionResult> Delete(short id)
        {
            //if (!IsStaff()) return Forbid();

            var success = await _categoryService.DeleteAsync(id);
            return success ? NoContent() : BadRequest("Category is linked to existing articles.");
        }
    }
}
