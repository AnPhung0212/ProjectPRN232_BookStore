using BookStore.BusinessObject.Models;
using BookStore.DataAccessObject.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccessObject.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly BookStoreDbContext _context;

        public OrderRepository(BookStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
            => await _context.Orders.Include(o => o.OrderDetails).ThenInclude(d => d.Product)
                                     .Include(o => o.Status)
                                     .Include(o => o.User)
                                     .ToListAsync();

        public async Task<Order?> GetByIdAsync(int id)
            => await _context.Orders.Include(o => o.OrderDetails).ThenInclude(d => d.Product)
                                     .Include(o => o.Status)
                                     .Include(o => o.User)
                                     .FirstOrDefaultAsync(o => o.OrderId == id);

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails).ThenInclude(d => d.Product)
                .Include(o => o.Status)
                .Include(o => o.User)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

    }
}