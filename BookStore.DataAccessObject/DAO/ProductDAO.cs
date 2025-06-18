using BookStore.BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccessObject.DAO
{
    public class ProductDAO
    {
        private readonly BookStoreDbContext _context;
        public ProductDAO(BookStoreDbContext context) {  _context = context; }
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                                 .Include(p => p.Category)
                                 .FirstOrDefaultAsync(p => p.ProductId == id);
        }
        // Thêm sản phẩm mới
        public async Task AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        // Cập nhật sản phẩm
        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        // Xóa sản phẩm
        public async Task DeleteProductAsync(int id)
        {
            var productToDelete = await _context.Products.FindAsync(id);
            if (productToDelete != null)
            {
                _context.Products.Remove(productToDelete);
                await _context.SaveChangesAsync();
            }
        }

        // Tìm kiếm sản phẩm theo 1 số thông tin của nó (ví dụ)
        public async Task<IEnumerable<Product>> SearchProductsByInformationAsync(string info)
        {
            return await _context.Products
                                 .Include(p => p.Category)
                                 .Where(p => p.Title.Contains(info)||p.Description.Contains(info) || p.Author.Contains(info) || p.Category.CategoryName.Contains(info))
                                 .ToListAsync();
        }
    }
}
