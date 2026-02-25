using BookStore.BusinessObject.DTO.UserDTOs;
using Microsoft.AspNetCore.Mvc;
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
            var client = _httpClientFactory.CreateClient("BookStoreAPI");
            var response = await client.PostAsJsonAsync("user/login", loginDto);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            var result = await response.Content.ReadFromJsonAsync<UserResponseDto>();
            if (result != null)
            {
                // Lưu token vào Session hoặc Cookie
                HttpContext.Session.SetString("JWToken", result.Token);
                HttpContext.Session.SetString("Username", result.User.Username);
                HttpContext.Session.SetString("UserEmail", result.User.Email);
                HttpContext.Session.SetString("UserId", result.User.UserId.ToString());
                HttpContext.Session.SetString("Role", result.User.RoleName ?? "Customer");
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Login failed.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index","Home");
        }

        // tiếp đến đăng ký
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserCreateDto registerDto)
        {
            registerDto.RoleId = 2;

            var client = _httpClientFactory.CreateClient("BookStoreAPI");
            var response = await client.PostAsJsonAsync("user/register", registerDto);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Đăng ký thất bại. Vui lòng kiểm tra thông tin.";
                return View();
            }

            return RedirectToAction("Login", "Account", new { successMessage = "Đăng ký thành công! Vui lòng đăng nhập." });
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

            var client = _httpClientFactory.CreateClient("BookStoreApi");

            // Giả sử API verify dùng GET /api/user/verify-email?token=...
            var response = await client.GetAsync($"/api/user/verify-email?token={Uri.EscapeDataString(token)}");

            string apiMessage = "Có lỗi xảy ra.";
            try
            {
                // API hiện đang trả { message = "..." }
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
