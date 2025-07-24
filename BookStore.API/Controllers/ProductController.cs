using BookStore.BusinessObject.DTO;
using BookStore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetByCategory(int categoryId)
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId);
            return Ok(products);
        }
        // GET: api/Product/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // POST: api/Product
        [HttpPost]
        [Authorize(Roles = "Admin")] // Nếu bạn dùng role
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _productService.AddProductAsync(dto);
            return Ok();
        }

        // PUT: api/Product/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDTO dto)
        {
            if (id != dto.ProductId)
                return BadRequest("Product ID mismatch.");

            try
            {
                await _productService.UpdateProductAsync(dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE: api/Product/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProductAsync(id);
            return NoContent();
        }

        // GET: api/Product/search?term=abc
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> Search(string term)
        {
            var products = await _productService.SearchProductsAsync(term);
            return Ok(products);
        }
    }
}
