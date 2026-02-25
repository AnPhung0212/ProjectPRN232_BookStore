using BookStore.BusinessObject.DTO.UserDTOs;
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
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _repo;

        public UserService(IGenericRepository<User> repo)
        {
            _repo = repo;
        }
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _repo.Entities.Include(u => u.Role).ToListAsync();
            return users.Select(ToDto);
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _repo.Entities.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserID == id);
            return user != null ? ToDto(user) : null;
        }

        public async Task<UserDto?> AuthenticateAsync(UserLoginDto loginDto)
        {
            var user = await _repo.Entities.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.Password == loginDto.Password && u.IsActive == true);
            return user != null ? ToDto(user) : null;
        }

        public async Task<IEnumerable<UserDto>> GetUsersByRoleIdAsync(int roleId)
        {
            var users = await _repo.Entities.Include(u => u.Role).Where(u => u.RoleID == roleId).ToListAsync();
            return users.Select(ToDto);
        }

        public async Task AddUserAsync(UserCreateDto userDto)
        {
            var user = ToEntity(userDto);
            await _repo.AddAsync(user);
        }

        public async Task<(bool IsSuccess, string Message)> UpdateUserWithPasswordAsync(int userId, UserUpdateDto dto)
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user == null) return (false, "User not found");

            if (user.Password != dto.Password)
                return (false, "Incorrect current password");

            if (!string.IsNullOrEmpty(dto.NewPassword))
            {
                if (dto.NewPassword != dto.ReconfirmPassword)
                    return (false, "New password and confirmation do not match");

                user.Password = dto.NewPassword;
            }

            user.Username = dto.Username;
            user.FullName = dto.FullName;
            user.Address = dto.Address;

            await _repo.UpdateAsync(user);
            return (true, "User updated successfully");
        }

        public async Task DeleteUserAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }
        //Mapper
        private UserDto ToDto(User user)
        {
            return new UserDto
            {
                UserId = user.UserID,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Address = user.Address,
                TotalAmount = user.TotalAmount,
                RoleName = user.Role?.RoleName
            };
        }
        private User ToEntity(UserCreateDto dto)
        {
            return new User
            {
                Username = dto.Username,
                Password = dto.Password,
                FullName = dto.FullName,
                Email = dto.Email,
                Address = dto.Address,
                RoleID = dto.RoleId,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
