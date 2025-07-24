using BookStore.BusinessObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
        Task<ProductDTO?> GetProductByIdAsync(int id);
        Task AddProductAsync(ProductCreateDTO product);
        Task UpdateProductAsync(UpdateProductDTO product);
        Task DeleteProductAsync(int id);
        Task<IEnumerable<ProductDTO>> SearchProductsAsync(string searchTerm);
        Task<IEnumerable<ProductDTO>> GetProductsByCategoryAsync(int id);
    }
}
