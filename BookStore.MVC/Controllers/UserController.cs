using BookStore.BusinessObject.DTO.UserDTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
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

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);

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
    }
}
