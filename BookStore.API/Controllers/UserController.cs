using BookStore.API.Helpers;
using BookStore.BusinessObject.DTO.UserDTOs;
using BookStore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BookStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        /// <summary>
        /// Đăng nhập vào hệ thống và nhận mã Token JWT
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto loginDto)
        {
            var user = await _userService.AuthenticateAsync(loginDto);
            if (user == null) return Unauthorized("Invalid credentials");

            var token = JwtHelper.GenerateJwtToken(user, _configuration);
            return Ok(new { token, user });
        }

        /// <summary>
        /// Đăng ký tài khoản người dùng mới
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserCreateDto dto)
        {
            await _userService.AddUserAsync(dto);
            return Ok("User created");
        }

        /// <summary>
        /// Lấy danh sách toàn bộ người dùng trong hệ thống (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một người dùng theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        /// <summary>
        /// Lấy danh sách người dùng dựa theo mã vai trò (Admin only)
        /// </summary>
        [HttpGet("role/{roleId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByRole(int roleId)
        {
            var users = await _userService.GetUsersByRoleIdAsync(roleId);
            return Ok(users);
        }

        /// <summary>
        /// Cập nhật thông tin tài khoản người dùng
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UserUpdateDto dto)
        {
            
                var result = await _userService.UpdateUserWithPasswordAsync(id, dto);
                if (!result.IsSuccess)
                    return BadRequest(result.Message);
                return Ok(result.Message);
            }
        

        /// <summary>
        /// Xóa tài khoản người dùng khỏi hệ thống (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok("Deleted");
        }

        // Helper để lấy userId từ token
        private int? GetUserIdFromToken()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null && int.TryParse(claim.Value, out var id) ? id : null;
        }
    }
}