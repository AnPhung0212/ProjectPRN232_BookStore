using BookStore.BusinessObject.Models;
using BookStore.DataAccessObject.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccessObject.IRepository
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByUserIdAsync(int userId);
        Task<CartItem?> GetCartItemAsync(int cartId, int productId);
        Task<List<CartItem>> GetCartItemsAsync(int cartId);
        Task AddCartItemAsync(CartItem item);
        Task UpdateCartItemQuantityAsync(int cartItemId, int newQuantity);
        Task DeleteCartItemAsync(int cartItemId);
        Task ClearCartAsync(int cartId);
        Task<Cart> CreateCartForUserAsync(int userId);
    }
}
