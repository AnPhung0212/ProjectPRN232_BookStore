using BookStore.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;

namespace BookStore.MVC.Controllers
{
    public class ProductMVCController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        public ProductMVCController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        public async Task<IActionResult> Index(string search, int? categoryId)
        {
            var client = _clientFactory.CreateClient("BookStoreApi");

            // Gọi API lấy danh mục
            var categoryResp = await client.GetAsync("Category");
            var categoryJson = await categoryResp.Content.ReadAsStringAsync();

            var allCategories = System.Text.Json.JsonSerializer.Deserialize<List<CategoryViewModel>>(categoryJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            // Gọi API tùy theo search hoặc categoryId
            HttpResponseMessage productResp;

            if (!string.IsNullOrEmpty(search))
            {
                productResp = await client.GetAsync($"Product/search?term={search}");
            }
            else if (categoryId.HasValue)
            {
                productResp = await client.GetAsync($"Product/category/{categoryId.Value}");
            }
            else
            {
                productResp = await client.GetAsync("Product");
            }

            var productJson = await productResp.Content.ReadAsStringAsync();

            var products = System.Text.Json.JsonSerializer.Deserialize<List<ProductViewModel>>(productJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            var viewModel = new ProductListViewModel
            {
                Products = products,
                Categories = allCategories,
                SearchKeyword = search,
                SelectedCategoryId = categoryId
            };

            return View(viewModel);
        }
        public async Task<IActionResult> Detail(int id)
        {
            var client = _clientFactory.CreateClient("BookStoreApi");
            var response = await client.GetAsync($"Product/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var json = await response.Content.ReadAsStringAsync();
            var product = System.Text.Json.JsonSerializer.Deserialize<ProductViewModel>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(product);
        }
    }
}
