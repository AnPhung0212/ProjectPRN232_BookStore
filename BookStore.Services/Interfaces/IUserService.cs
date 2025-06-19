using BookStore.BusinessObject.DTO.UserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto?> AuthenticateAsync(UserLoginDto loginDto);
        Task<IEnumerable<UserDto>> GetUsersByRoleIdAsync(int roleId);
        Task AddUserAsync(UserCreateDto userDto);
        Task UpdateUserAsync(int userId, UserCreateDto userDto);
        Task DeleteUserAsync(int id);
    }
}
