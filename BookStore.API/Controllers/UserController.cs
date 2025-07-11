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

        // POST: api/User/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto loginDto)
        {
            var user = await _userService.AuthenticateAsync(loginDto);
            if (user == null) return Unauthorized("Invalid credentials");

            var token = JwtHelper.GenerateJwtToken(user, _configuration);
            return Ok(new { token, user });
        }

        // POST: api/User/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserCreateDto dto)
        {
            await _userService.AddUserAsync(dto);
            return Ok("User created");
        }

        // GET: api/User
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // GET: api/User/role/2
        [HttpGet("role/{roleId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByRole(int roleId)
        {
            var users = await _userService.GetUsersByRoleIdAsync(roleId);
            return Ok(users);
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, UserCreateDto dto)
        {
            await _userService.UpdateUserAsync(id, dto);
            return Ok("Updated");
        }

        // DELETE: api/User/5
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