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
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _productRepo;

        public ProductService(IGenericRepository<Product> productRepo)
        {
            _productRepo = productRepo;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            var products = await _productRepo.Entities.Include(p => p.Category).ToListAsync();
            return products.Select(p => MapToDTO(p));
        }

        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            var product = await _productRepo.Entities.Include(p => p.Category).FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null) return null;

            return MapToDTO(product);
        }

        public async Task AddProductAsync(ProductCreateDTO productDto)
        {
            var product = MapToEntity(productDto);
            await _productRepo.AddAsync(product);
        }

        public async Task UpdateProductAsync(UpdateProductDTO dto)
        {
            var existingProduct = await _productRepo.GetByIdAsync(dto.ProductId);
            if (existingProduct == null)
            {
                throw new Exception("Product not found");
            }
            // Cập nhật có điều kiện (nếu khác null thì mới cập nhật)
            if (dto.CategoryId.HasValue)
                existingProduct.CategoryID = dto.CategoryId.Value;

            if (!string.IsNullOrWhiteSpace(dto.Title))
                existingProduct.Title = dto.Title;

            if (!string.IsNullOrWhiteSpace(dto.Author))
                existingProduct.Author = dto.Author;

            if (dto.Price.HasValue)
                existingProduct.Price = dto.Price.Value;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                existingProduct.Description = dto.Description;

            if (dto.Stock.HasValue)
                existingProduct.Stock = dto.Stock.Value;

            if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
                existingProduct.ImageURL = dto.ImageUrl;
            
            await _productRepo.UpdateAsync(existingProduct);
        }

        public async Task DeleteProductAsync(int id)
        {
            await _productRepo.DeleteAsync(id);
        }

        public async Task<IEnumerable<ProductDTO>> SearchProductsAsync(string searchTerm)
        {
            var products = await _productRepo.Entities
                .Include(p => p.Category)
                .Where(p => p.Title.Contains(searchTerm) || p.Author.Contains(searchTerm))
                .ToListAsync();
            return products.Select(p => MapToDTO(p));
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _productRepo.Entities
                .Include(p => p.Category)
                .Where(p => p.CategoryID == categoryId)
                .ToListAsync();
            return products.Select(p => MapToDTO(p));
        }

        // Mapping helpers
        private ProductDTO MapToDTO(Product p) => new ProductDTO
        {
            ProductId = p.ProductID,
            CategoryId = p.CategoryID,
            Title = p.Title,
            Author = p.Author,
            Price = p.Price,
            Description = p.Description,
            Stock = p.Stock,
            ImageUrl = p.ImageURL,
            CategoryName = p.Category?.CategoryName
        };

        private Product MapToEntity(ProductCreateDTO dto) => new Product
        {
            CategoryID = dto.CategoryId,
            Title = dto.Title,
            Author = dto.Author,
            Price = dto.Price,
            Description = dto.Description,
            Stock = dto.Stock,
            ImageURL = dto.ImageUrl
        };
    }
}
