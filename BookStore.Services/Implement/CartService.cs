using BookStore.BusinessObject.DTO;
using BookStore.BusinessObject.Models;
using BookStore.DataAccessObject.IRepository;
using BookStore.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Services.Implement
{
    public class CartService : ICartService
    {
        private readonly IGenericRepository<Cart> _cartRepo;
        private readonly IGenericRepository<CartItem> _cartItemRepo;

        public CartService(IGenericRepository<Cart> cartRepo, IGenericRepository<CartItem> cartItemRepo)
        {
            _cartRepo = cartRepo;
            _cartItemRepo = cartItemRepo;
        }

        public async Task<CartDTO?> GetCartByUserIdAsync(int userId)
        {
            var cart = await _cartRepo.Entities
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            return cart == null ? null : ToDto(cart);
        }

        public async Task<List<CartItemDTO>> GetCartItemsAsync(int cartId)
        {
            var items = await _cartItemRepo.Entities
                .Include(ci => ci.Product)
                .Where(ci => ci.CartId == cartId)
                .ToListAsync();

            return items.Select(ci => new CartItemDTO
            {
                CartItemId = ci.CartItemId,
                ProductId = ci.ProductId,
                ProductName = ci.Product?.Title,
                Quantity = ci.Quantity,
                AddedDate = ci.AddedDate,
                UnitPrice = ci.Product?.Price,
                ImageUrl = ci.Product?.ImageUrl
            }).ToList();
        }

        public async Task AddCartItemAsync(int userId, int productId, int quantity)
        {
            var cart = await _cartRepo.Entities.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId, CreatedDate = DateTime.UtcNow };
                await _cartRepo.AddAsync(cart);
            }

            var existingItem = await _cartItemRepo.Entities.FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                await _cartItemRepo.UpdateAsync(existingItem);
            }
            else
            {
                var newItem = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = productId,
                    Quantity = quantity,
                    AddedDate = DateTime.UtcNow
                };
                await _cartItemRepo.AddAsync(newItem);
            }
        }

        public async Task UpdateCartItemQuantityAsync(int cartItemId, int newQuantity)
        {
            var item = await _cartItemRepo.GetByIdAsync(cartItemId);
            if (item != null)
            {
                item.Quantity = newQuantity;
                await _cartItemRepo.UpdateAsync(item);
            }
        }

        public async Task DeleteCartItemAsync(int cartItemId)
        {
            await _cartItemRepo.DeleteAsync(cartItemId);
        }

        public async Task ClearCartAsync(int cartId)
        {
            var items = await _cartItemRepo.Entities.Where(ci => ci.CartId == cartId).ToListAsync();
            foreach (var item in items)
            {
                await _cartItemRepo.DeleteAsync(item.CartItemId);
            }
        }

        public CartDTO ToDto(Cart cart)
        {
            return new CartDTO
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                CreatedDate = cart.CreatedDate,
                Items = cart.CartItems?.Select(ci => new CartItemDTO
                {
                    CartItemId = ci.CartItemId,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product?.Title,
                    Quantity = ci.Quantity,
                    AddedDate = ci.AddedDate,
                    UnitPrice = ci.Product?.Price,
                    ImageUrl = ci.Product?.ImageUrl
                }).ToList() ?? new List<CartItemDTO>()
            };
        }
    }
}
