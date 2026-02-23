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
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<Category> _categoryRepo;

        public CategoryService(IGenericRepository<Category> categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllAsync()
        {
            var categories = await _categoryRepo.GetAllAsync();
            return categories.Select(c => MapToDTO(c));
        }

        public async Task<CategoryDTO?> GetByIdAsync(int id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            return category != null ? MapToDTO(category) : null;
        }

        public async Task AddAsync(CategoryDTO dto)
        {
            var category = MapToEntity(dto);
            await _categoryRepo.AddAsync(category);
        }

        public async Task UpdateAsync(CategoryDTO dto)
        {
            var existing = await _categoryRepo.GetByIdAsync(dto.CategoryId);
            if (existing == null) throw new Exception("Category not found");

            existing.CategoryName = dto.CategoryName;
            existing.Description = dto.Description;
            await _categoryRepo.UpdateAsync(existing);
        }

        public Task DeleteAsync(int id) => _categoryRepo.DeleteAsync(id);



        // Mapping helpers
        private static CategoryDTO MapToDTO(Category c) => new CategoryDTO
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName
        };

        private static Category MapToEntity(CategoryDTO dto) => new Category
        {
            CategoryId = dto.CategoryId,
            CategoryName = dto.CategoryName
        };
    }
}
