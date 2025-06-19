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
    public class UserRepository : IUserRepository
    {
        private readonly BookStoreDbContext _context;

        public UserRepository(BookStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync() =>
            await _context.Users.Include(u => u.Role).ToListAsync();

        public async Task<User?> GetByIdAsync(int id) =>
            await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == id);

        public async Task<User?> GetByUsernamePasswordAsync(string email, string password) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Username == email && u.Password == password);

        public async Task<IEnumerable<User>> GetUsersByRoleIdAsync(int roleId) =>
            await _context.Users.Where(u => u.RoleId == roleId).ToListAsync();

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
