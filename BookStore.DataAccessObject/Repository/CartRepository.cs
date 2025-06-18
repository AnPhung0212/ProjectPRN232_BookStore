using BookStore.BusinessObject.Models;
using BookStore.DataAccessObject.DAO;
using BookStore.DataAccessObject.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccessObject.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly CartDAO _dao;

        public CartRepository(BookStoreDbContext context)
        {
            _dao = new CartDAO(context);
        }

        public Task<Cart?> GetCartByUserIdAsync(int userId) => _dao.GetCartByUserIdAsync(userId);

        public Task<CartItem?> GetCartItemAsync(int cartId, int productId) => _dao.GetCartItemAsync(cartId, productId);

        public Task<List<CartItem>> GetCartItemsAsync(int cartId) => _dao.GetCartItemsAsync(cartId);

        public Task AddCartItemAsync(CartItem item) => _dao.AddCartItemAsync(item);

        public Task UpdateCartItemQuantityAsync(int cartItemId, int newQuantity) => _dao.UpdateCartItemQuantityAsync(cartItemId, newQuantity);

        public Task DeleteCartItemAsync(int cartItemId) => _dao.DeleteCartItemAsync(cartItemId);

        public Task ClearCartAsync(int cartId) => _dao.ClearCartAsync(cartId);

        public Task<Cart> CreateCartForUserAsync(int userId) => _dao.CreateCartForUserAsync(userId);
    }
}
