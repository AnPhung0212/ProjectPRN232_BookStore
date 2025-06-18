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
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;

        public ProductService(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }
        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            var products = await _productRepo.GetAllProductsAsync();
            return products.Select(p => MapToDTO(p));
        }

        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            var product = await _productRepo.GetProductByIdAsync(id);
            return product == null ? null : MapToDTO(product);
        }

        public async Task AddProductAsync(ProductDTO productDto)
        {
            var product = MapToEntity(productDto);
            await _productRepo.AddProductAsync(product);
        }

        public async Task UpdateProductAsync(ProductDTO productDto)
        {
            var product = MapToEntity(productDto);
            await _productRepo.UpdateProductAsync(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            await _productRepo.DeleteProductAsync(id);
        }

        public async Task<IEnumerable<ProductDTO>> SearchProductsAsync(string searchTerm)
        {
            var products = await _productRepo.SearchProductsAsync(searchTerm);
            return products.Select(p => MapToDTO(p));
        }

        // Mapping helpers
        private ProductDTO MapToDTO(Product p) => new ProductDTO
        {
            ProductId = p.ProductId,
            CategoryId = p.CategoryId,
            Title = p.Title,
            Author = p.Author,
            Price = p.Price,
            Description = p.Description,
            Stock = p.Stock,
            ImageUrl = p.ImageUrl
        };

        private Product MapToEntity(ProductDTO dto) => new Product
        {
            ProductId = dto.ProductId,
            CategoryId = dto.CategoryId,
            Title = dto.Title,
            Author = dto.Author,
            Price = dto.Price,
            Description = dto.Description,
            Stock = dto.Stock,
            ImageUrl = dto.ImageUrl
        };
    }
}
