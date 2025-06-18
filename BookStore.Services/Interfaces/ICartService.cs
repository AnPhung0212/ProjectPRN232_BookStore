using BookStore.BusinessObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartDTO?> GetCartByUserIdAsync(int userId);
        Task<List<CartItemDTO>> GetCartItemsAsync(int cartId);
        Task AddCartItemAsync(int userId, int productId, int quantity);
        Task UpdateCartItemQuantityAsync(int cartItemId, int newQuantity);
        Task DeleteCartItemAsync(int cartItemId);
        Task ClearCartAsync(int cartId);
    }
}
