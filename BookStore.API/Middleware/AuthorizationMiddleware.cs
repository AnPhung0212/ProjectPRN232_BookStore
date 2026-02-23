using BookStore.BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims; // Correct for ClaimTypes

namespace BookStore.API.Middleware
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public AuthorizationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context, BookStoreDbContext dbContext)
        {
            // 1. Kiểm tra Route công khai trước
            if (EnableAuthorizationRoute.IsPublicRoute(context))
            {
                await _next(context);
                return;
            }

            // 2. Kiểm tra Master Key (Dùng cho các ứng dụng nội bộ/Admin tool gọi tới)
            var masterKey = context.Request.Headers["X-Master-Key"].FirstOrDefault();
            if (!string.IsNullOrEmpty(masterKey) && masterKey == _configuration["Authorization:MasterKey"])
            {
                await _next(context);
                return;
            }

            // 3. Kiểm tra JWT Token
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            var token = authHeader?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                var userId = await ValidateTokenAndGetUserId(token, dbContext);
                if (userId != null)
                {
                    // Gán User vào Items để Controller có thể lấy nhanh
                    context.Items["User"] = await dbContext.Users.FindAsync(userId);
                    await _next(context);
                    return;
                }
            }

            // Nếu không đạt điều kiện nào thì trả về 401
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { message = "Unauthorized - Missing or invalid token/key" });
        }

        private async Task<int?> ValidateTokenAndGetUserId(string token, BookStoreDbContext dbContext)
        {
            try
            {
                var jwtHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Key"]!);

                jwtHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["JwtSettings:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                return userIdClaim != null ? int.Parse(userIdClaim) : null;
            }
            catch
            {
                return null;
            }
        }
    }
}