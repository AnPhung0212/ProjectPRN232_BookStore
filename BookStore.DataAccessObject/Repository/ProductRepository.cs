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
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDAO _productDAO;
        public ProductRepository(ProductDAO productDAO)
        {
            _productDAO = productDAO;
        }
        public Task AddProductAsync(Product product)
        {
          return _productDAO.AddProductAsync(product);
        }

        public Task DeleteProductAsync(int id)
        {
            return _productDAO.DeleteProductAsync(id);
        }

        public Task<IEnumerable<Product>> GetAllProductsAsync()
        {
             return _productDAO.GetAllProductsAsync();
        }

        public Task<Product?> GetProductByIdAsync(int id)
        {
             return _productDAO.GetProductByIdAsync(id);
        }

        public Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            return _productDAO.SearchProductsByInformationAsync(searchTerm);
        }

        public Task UpdateProductAsync(Product product)
        {
            return _productDAO.UpdateProductAsync(product);
        }
    }
}
