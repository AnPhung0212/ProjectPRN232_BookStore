using BookStore.BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccessObject.DAO
{
    public class CartDAO
    {
        private readonly BookStoreDbContext _context;

        public CartDAO(BookStoreDbContext context)
        {
            _context = context;
        }
        // Lấy giỏ hàng theo user
        public async Task<Cart?> GetCartByUserIdAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems!)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }
        // Lấy cart item theo cart và product
        public async Task<CartItem?> GetCartItemAsync(int cartId, int productId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
        }
        // Lấy toàn bộ cart item trong giỏ hàng
        public async Task<List<CartItem>> GetCartItemsAsync(int cartId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.CartId == cartId)
                .ToListAsync();
        }
        // Thêm sản phẩm vào giỏ
        public async Task AddCartItemAsync(CartItem item)
        {
            await _context.CartItems.AddAsync(item);
            await _context.SaveChangesAsync();
        }
        // Cập nhật số lượng sản phẩm
        public async Task UpdateCartItemAsync(CartItem item)
        {
            _context.CartItems.Update(item);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateCartItemQuantityAsync(int cartItemId, int newQuantity)
        {
            var item = await _context.CartItems.FindAsync(cartItemId);
            if (item != null)
            {
                item.Quantity = newQuantity;
                _context.CartItems.Update(item); // hoặc không cần dòng này
                await _context.SaveChangesAsync();
            }
        }
        // Xoá một sản phẩm khỏi giỏ
        public async Task DeleteCartItemAsync(int cartItemId)
        {
            var item = await _context.CartItems.FindAsync(cartItemId);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
        // Xoá toàn bộ sản phẩm trong giỏ
        public async Task ClearCartAsync(int cartId)
        {
            var items = _context.CartItems.Where(ci => ci.CartId == cartId);
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
        // Tạo mới giỏ hàng cho user nếu chưa có
        public async Task<Cart> CreateCartForUserAsync(int userId)
        {
            var existingCart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            if (existingCart != null)
            {
                return existingCart;
            }

            var newCart = new Cart
            {
                UserId = userId,
                CreatedDate = DateTime.UtcNow
            };

            await _context.Carts.AddAsync(newCart);
            await _context.SaveChangesAsync();

            return newCart;
        }

    }
}
