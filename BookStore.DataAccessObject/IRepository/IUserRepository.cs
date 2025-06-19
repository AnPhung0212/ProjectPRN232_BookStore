using BookStore.BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccessObject.IRepository
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        //login
        Task<User?> GetByUsernamePasswordAsync(string username, string password);
        Task<IEnumerable<User>> GetUsersByRoleIdAsync(int roleId);
        //register
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
    }
}
