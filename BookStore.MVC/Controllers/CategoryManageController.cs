using BookStore.BusinessObject.DTO;
using BookStore.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace BookStore.MVC.Controllers
{
    public class CategoryManageController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CategoryManageController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("BookStoreAPI");
            var categories = await client.GetFromJsonAsync<List<CategoryDTO>>("Category");
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("BookStoreAPI");
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsJsonAsync("Category", new CategoryDTO
            {
                CategoryName = model.CategoryName
            });

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Tạo danh mục thành công";
                return RedirectToAction("Index");
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Thất bại: {error}");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("BookStoreAPI");
            var response = await client.GetAsync($"Category/{id}");

            if (!response.IsSuccessStatusCode) return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var dto = JsonConvert.DeserializeObject<CategoryDTO>(json);

            var model = new CategoryDTO
            {
                CategoryId = dto.CategoryId,
                CategoryName = dto.CategoryName
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CategoryDTO model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("BookStoreAPI");
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"Category/{model.CategoryId}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Cập nhật danh mục thành công!";
                return RedirectToAction("Index");
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Lỗi: {error}");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("BookStoreAPI");
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"Category/{id}");
            if (response.IsSuccessStatusCode)
                TempData["Success"] = "Xóa danh mục thành công!";
            else
                TempData["Error"] = "Xóa thất bại";

            return RedirectToAction("Index");
        }
    }
}
