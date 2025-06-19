using BookStore.BusinessObject.DTO.UserDTOs;
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
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _repo.GetAllAsync();
            return users.Select(ToDto);
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _repo.GetByIdAsync(id);
            return user != null ? ToDto(user) : null;
        }

        public async Task<UserDto?> AuthenticateAsync(UserLoginDto loginDto)
        {
            var user = await _repo.GetByUsernamePasswordAsync(loginDto.Username, loginDto.Password);
            return user != null ? ToDto(user) : null;
        }

        public async Task<IEnumerable<UserDto>> GetUsersByRoleIdAsync(int roleId)
        {
            var users = await _repo.GetUsersByRoleIdAsync(roleId);
            return users.Select(ToDto);
        }

        public async Task AddUserAsync(UserCreateDto userDto)
        {
            var user = ToEntity(userDto);
            await _repo.AddAsync(user);
        }

        public async Task UpdateUserAsync(int userId, UserCreateDto userDto)
        {
            var existing = await _repo.GetByIdAsync(userId);
            if (existing != null)
            {
                existing.Username = userDto.Username;
                existing.Password = userDto.Password;
                existing.FullName = userDto.FullName;
                existing.Email = userDto.Email;
                existing.Address = userDto.Address;
                existing.RoleId = userDto.RoleId;

                await _repo.UpdateAsync(existing);
            }
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
                UserId = user.UserId,
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
                RoleId = dto.RoleId,
                CreatedAt = DateTime.UtcNow
            };
        }
        }
}
