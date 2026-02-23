using BookStore.BusinessObject.DTO;
using BookStore.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>
        /// Lấy thông tin giỏ hàng của người dùng hiện tại
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<CartDTO>> GetCart()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var cart = await _cartService.GetCartByUserIdAsync(userId.Value);
            return cart == null ? NotFound() : Ok(cart);
        }

        /// <summary>
        /// Lấy danh sách các sản phẩm trong giỏ hàng
        /// </summary>
        [HttpGet("items")]
        public async Task<ActionResult<List<CartItemDTO>>> GetCartItems()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var cart = await _cartService.GetCartByUserIdAsync(userId.Value);
            if (cart == null) return NotFound();

            var items = await _cartService.GetCartItemsAsync(cart.CartId);
            return Ok(items);
        }

        /// <summary>
        /// Thêm sản phẩm vào giỏ hàng
        /// </summary>
        [HttpPost("add")]
        public async Task<IActionResult> AddItem([FromBody] AddToCartRequest request)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            await _cartService.AddCartItemAsync(userId.Value, request.ProductId, request.Quantity);
            return Ok(new { message = "Item added to cart" });
        }

        /// <summary>
        /// Cập nhật số lượng của một sản phẩm trong giỏ hàng
        /// </summary>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateCartQuantityRequest request)
        {
            await _cartService.UpdateCartItemQuantityAsync(request.CartItemId, request.NewQuantity);
            return Ok(new { message = "Quantity updated" });
        }

        /// <summary>
        /// Xóa một sản phẩm khỏi giỏ hàng
        /// </summary>
        [HttpDelete("item/{cartItemId}")]
        public async Task<IActionResult> DeleteItem(int cartItemId)
        {
            await _cartService.DeleteCartItemAsync(cartItemId);
            return Ok(new { message = "Item removed from cart" });
        }

        /// <summary>
        /// Xóa toàn bộ sản phẩm trong giỏ hàng
        /// </summary>
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var cart = await _cartService.GetCartByUserIdAsync(userId.Value);
            if (cart == null) return NotFound();

            await _cartService.ClearCartAsync(cart.CartId);
            return Ok(new { message = "Cart cleared" });
        }

        // Helper to get userId from JWT
        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null && int.TryParse(userIdClaim.Value, out var id) ? id : null;
        }
    }

    // Request DTOs for POST/PUT
    public class AddToCartRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateCartQuantityRequest
    {
        public int CartItemId { get; set; }
        public int NewQuantity { get; set; }
    }
}
