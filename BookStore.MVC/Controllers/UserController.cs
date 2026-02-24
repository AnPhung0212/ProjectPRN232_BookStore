using BookStore.BusinessObject.DTO.UserDTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BookStore.MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;

        public UserController(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
        }
        public async Task<IActionResult> Index()
        {
            int userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");

            if (userId == 0)
                return RedirectToAction("Login", "Account");

            var client = _clientFactory.CreateClient("BookStoreAPI");

            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync($"User/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Không thể tải thông tin người dùng.";
                return RedirectToAction("Index", "Home");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var user = System.Text.Json.JsonSerializer.Deserialize<UserDto>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            ViewBag.UserId = userId;

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateUser()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login", "Account");
            }

            if (!int.TryParse(userIdStr, out int userId))
            {
                return View("Error");
            }

            var client = _clientFactory.CreateClient("BookStoreAPI");

            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync($"/api/User/{userId}");
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = $"Lỗi khi gọi API: {response.StatusCode}";
                return View("Error");
            }

            var content = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<UserDto>(content);

            if (user == null)
            {
                ViewBag.Error = "Không tìm thấy thông tin người dùng.";
                return View("Error");
            }

            var model = new UserUpdateDto
            {
                Username = user.Username,
                FullName = user.FullName,
                Address = user.Address
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateUser(UserUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login", "Account");
            }

            if (!int.TryParse(userIdStr, out int userId))
            {
                ViewBag.Error = "Lỗi định dạng UserId.";
                return View("Error");
            }

            var client = _clientFactory.CreateClient("BookStoreAPI");

            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/User/{userId}", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ViewBag.Error = error;
                return View(model);
            }

            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Index"); // hoặc ở lại trang cập nhật: return View(model);
        }


    }
}
