using BookStore.BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccessObject.DAO
{
    public class CategoryDAO
    {
        private readonly BookStoreDbContext _context;

        public CategoryDAO(BookStoreDbContext context)
        {
            _context = context;
        }

        // Lấy tất cả category
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        // Lấy category theo id
        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        // Thêm mới
        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        // Cập nhật
        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        // Xoá theo ID
        public async Task DeleteAsync(int id)
        {
            var cat = await _context.Categories.FindAsync(id);
            if (cat != null)
            {
                _context.Categories.Remove(cat);
                await _context.SaveChangesAsync();
            }
        }

        // Tìm kiếm theo tên
        public async Task<IEnumerable<Category>> SearchByNameAsync(string keyword)
        {
            return await _context.Categories
                .Where(c => c.CategoryName.Contains(keyword))
                .ToListAsync();
        }
    }
}
