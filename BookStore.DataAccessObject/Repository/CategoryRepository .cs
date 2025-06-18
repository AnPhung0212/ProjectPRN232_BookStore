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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CategoryDAO _categoryDAO;

        public CategoryRepository(CategoryDAO categoryDAO)
        {
            _categoryDAO = categoryDAO;
        }

        public Task<IEnumerable<Category>> GetAllAsync() => _categoryDAO.GetAllAsync();

        public Task<Category?> GetByIdAsync(int id) => _categoryDAO.GetByIdAsync(id);

        public Task AddAsync(Category category) => _categoryDAO.AddAsync(category);

        public Task UpdateAsync(Category category) => _categoryDAO.UpdateAsync(category);

        public Task DeleteAsync(int id) => _categoryDAO.DeleteAsync(id);

    }
}
