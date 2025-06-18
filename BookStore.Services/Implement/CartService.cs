using BookStore.BusinessObject.DTO;
using BookStore.BusinessObject.Models;
using BookStore.DataAccessObject.IRepository;
using BookStore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Services.Implement
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _repository;

        public CartService(ICartRepository repository)
        {
            _repository = repository;
        }

        public async Task<CartDTO?> GetCartByUserIdAsync(int userId)
        {
            var cart = await _repository.GetCartByUserIdAsync(userId);
            return cart == null ? null : ToDto(cart);
        }

        public async Task<List<CartItemDTO>> GetCartItemsAsync(int cartId)
        {
            var items = await _repository.GetCartItemsAsync(cartId);
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
            var cart = await _repository.CreateCartForUserAsync(userId);

            var existingItem = await _repository.GetCartItemAsync(cart.CartId, productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                await _repository.UpdateCartItemQuantityAsync(existingItem.CartItemId, existingItem.Quantity);
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
                await _repository.AddCartItemAsync(newItem);
            }
        }

        public Task UpdateCartItemQuantityAsync(int cartItemId, int newQuantity)
            => _repository.UpdateCartItemQuantityAsync(cartItemId, newQuantity);

        public Task DeleteCartItemAsync(int cartItemId)
            => _repository.DeleteCartItemAsync(cartItemId);

        public Task ClearCartAsync(int cartId)
            => _repository.ClearCartAsync(cartId);
        public  CartDTO ToDto(Cart cart)
        {
            return new CartDTO
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                CreatedDate = cart.CreatedDate,
                Items = cart.CartItems.Select(ci => new CartItemDTO
                {
                    CartItemId = ci.CartItemId,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product?.Title,
                    Quantity = ci.Quantity,
                    AddedDate = ci.AddedDate,
                    UnitPrice = ci.Product?.Price,
                    ImageUrl = ci.Product?.ImageUrl
                }).ToList()
            };
        }
    }
}
