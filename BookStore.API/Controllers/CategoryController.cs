using BookStore.BusinessObject.DTO;
using BookStore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        // GET: api/Category/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        // POST: api/Category
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CategoryDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _categoryService.AddAsync(dto);
            return Ok(new { message = "Category created successfully" });
        }

        // PUT: api/Category/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryDTO dto)
        {
            if (id != dto.CategoryId)
                return BadRequest("Category ID mismatch");

            try
            {
                await _categoryService.UpdateAsync(dto);
                return Ok(new { message = "Category updated successfully" });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE: api/Category/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.DeleteAsync(id);
            return Ok(new { message = "Category deleted successfully" });
        }
    }
}
