using BookStore.BusinessObject.DTO.UserDTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;

namespace BookStore.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Login(string? successMessage = null)
        {
            ViewBag.SuccessMessage = successMessage;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }

            var client = _httpClientFactory.CreateClient("BookStoreAPI");
            var response = await client.PostAsJsonAsync("user/login", loginDto);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Invalid email or password.";
                return View(loginDto);
            }

            var result = await response.Content.ReadFromJsonAsync<UserResponseDto>();
            if (result != null)
            {
                HttpContext.Session.SetString("JWToken", result.Token);
                HttpContext.Session.SetString("Username", result.User.Username);
                HttpContext.Session.SetString("UserEmail", result.User.Email);
                HttpContext.Session.SetString("UserId", result.User.UserId.ToString());
                HttpContext.Session.SetString("Role", result.User.RoleName ?? "Customer");
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Login failed.";
            return View(loginDto);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserCreateDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return View(registerDto);
            }

            registerDto.RoleId = 2;

            var client = _httpClientFactory.CreateClient("BookStoreAPI");
            var response = await client.PostAsJsonAsync("user/register", registerDto);

            var content = await response.Content.ReadAsStringAsync();
            string apiMessage = "Đăng ký thất bại. Vui lòng kiểm tra thông tin.";

            try
            {
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.TryGetProperty("message", out var msgProp))
                {
                    apiMessage = msgProp.GetString() ?? apiMessage;
                }
            }
            catch
            {
                // ignore parse error, giữ message mặc định
            }

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = apiMessage;
                return View(registerDto);
            }

            return RedirectToAction("Login", "Account", new { successMessage = apiMessage });
        }

        // GET: /Account/VerifyEmail?token=...
        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                ViewBag.Success = false;
                ViewBag.Message = "Token không hợp lệ.";
                return View();
            }

            var client = _httpClientFactory.CreateClient("BookStoreAPI");

            var response = await client.PostAsJsonAsync("user/verify-email", token);

            string apiMessage = "Có lỗi xảy ra.";
            try
            {
                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.TryGetProperty("message", out var msgProp))
                {
                    apiMessage = msgProp.GetString() ?? apiMessage;
                }
                else
                {
                    apiMessage = content;
                }
            }
            catch
            {
                // ignore parse error, dùng message mặc định
            }

            ViewBag.Success = response.IsSuccessStatusCode;
            ViewBag.Message = apiMessage;
            return View();
        }
    }
}
