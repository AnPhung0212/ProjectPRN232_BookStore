using BookStore.BusinessObject.Config;
using BookStore.BusinessObject.Models;
using BookStore.DataAccessObject.IRepository;
using BookStore.Helper.Template;
using BookStore.Services.Catalog.MailServices;
using BookStore.Services.Implement.Input;
using BookStore.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static BookStore.BusinessObject.Config.MailSetting;

namespace BookStore.Services.Implement
{
    public class RegisterService : IRegisterService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IMailService _mailService;
        private readonly JwtEmailSettings _jwtEmailSettings;
        private readonly MailSettings _mailSettings;
        private readonly ILogger<RegisterService> _logger;

        public RegisterService(
            IGenericRepository<User> userRepository,
            IMailService mailService,
            IOptions<JwtEmailSettings> jwtEmailSettings,
            IOptions<MailSettings> mailSettings,
            ILogger<RegisterService> logger)
        {
            _userRepository = userRepository;
            _mailService = mailService;
            _jwtEmailSettings = jwtEmailSettings.Value;
            _mailSettings = mailSettings.Value;
            _logger = logger;
        }

public async Task<string> RegisterUserAsync(RegisterInput input)
{
    if (input == null)
        return "Không đc để trống thông tin đăng ký";

    if (!IsValidPassword(input.Password))
        return "Mật khẩu yếu.";

    // check trùng username / email
    var existedUser = await _userRepository.Entities
        .FirstOrDefaultAsync(u => u.Username == input.UserName || u.Email == input.Email);

    if (existedUser != null)
        return "Username hoặc Email đã tồn tại.";

    var user = new User
    {
        Username = input.UserName,
        Password = input.Password, // TODO: hash
        Email = input.Email,
        CreatedAt = DateTime.UtcNow,
        Address = input.Address,
        IsActive = false
    };

    // LƯU USER TRƯỚC để có UserID
    await _userRepository.AddAsync(user);

    // Generate token sau khi UserID đã có
    var token = GenerateEmailToken(user.UserID);
    string subject = "Xác nhận email đăng ký tài khoản BookStore";

    // Đảm bảo BaseUrl trỏ đúng MVC: vd https://.../Account/VerifyEmail
    string confirmUrl = $"{_mailSettings.BaseUrl}?token={token}";
    string body = EmailTemplate.BodyRegister.Replace("{{confirmUrl}}", confirmUrl);

    try
    {
        await _mailService.SendEmailAsync(user.Email!, subject, body);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error sending verification email to {Email}", user.Email);
        // Tùy policy: user đã được tạo nhưng chưa verify được qua email
        return "Đăng ký thành công nhưng gửi email xác nhận thất bại. Vui lòng thử lại sau.";
    }

    return "Đăng ký thành công. Vui lòng kiểm tra email để xác nhận tài khoản.";
}
        public async Task<(int StatusCode, string Message)> VerifyUserAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return (StatusCodes.Status401Unauthorized, "Token is required.");

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtEmailSettings.SecretKey);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _jwtEmailSettings.Issuer,
                    ValidAudience = _jwtEmailSettings.Audience,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                    return (StatusCodes.Status401Unauthorized, "Token không hợp lệ.");

                var user = await _userRepository
                    .Entities
                    .FirstOrDefaultAsync(u => u.UserID == userId);

                if (user == null)
                    return (StatusCodes.Status404NotFound, "User not found.");

                if (user.IsActive)
                    return (StatusCodes.Status200OK, "User is already verified.");

                user.IsActive = true;
                await _userRepository.UpdateAsync(user);

                return (StatusCodes.Status200OK, "User verified successfully.");
            }
            catch (SecurityTokenExpiredException)
            {
                return (StatusCodes.Status401Unauthorized, "Token has expired.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Verification error");
                return (StatusCodes.Status500InternalServerError, "An error occurred during verification.");
            }
        }

        private static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;
            bool hasLower = password.Any(char.IsLower);
            bool hasUpper = password.Any(char.IsUpper);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(c => !char.IsLetterOrDigit(c));
            return hasLower && hasUpper && hasDigit && hasSpecial;
        }

        private string GenerateEmailToken(int userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtEmailSettings.SecretKey);

            var claims = new[]
            {
                new Claim("userId", userId.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _jwtEmailSettings.Issuer,
                Audience = _jwtEmailSettings.Audience,
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
