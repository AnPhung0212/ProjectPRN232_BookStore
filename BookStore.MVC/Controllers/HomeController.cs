using BookStore.MVC.Models;
using BookStore.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.Json;
using System.Net.Http;

namespace BookStore.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        public HomeController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;

        }
        public async Task<IActionResult> Index()
        {
            var client = _clientFactory.CreateClient("BookStoreApi");

            var productResponse = await client.GetAsync("Product");
            if (!productResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("API ERROR: " + productResponse.StatusCode);
                return View(new HomePageViewModel()); // tránh null
            }

            var productJson = await productResponse.Content.ReadAsStringAsync();
            Console.WriteLine("== JSON FROM API ==\n" + productJson);

            List<ProductViewModel>? allProducts = new();
            try
            {
                allProducts = System.Text.Json.JsonSerializer.Deserialize<List<ProductViewModel>>(
                    productJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("LỖI JSON: " + ex.Message);
            }

            allProducts ??= new List<ProductViewModel>();

            var viewModel = new HomePageViewModel
            {
                AllProducts = allProducts,
                Top3ExpensiveProducts = allProducts.OrderByDescending(p => p.Price).Take(4).ToList(),
                Top3NewestProducts = allProducts.OrderByDescending(p => p.ProductId).Take(3).ToList()
            };

            return View(viewModel);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
