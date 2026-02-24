using BookStore.BusinessObject.DTO;
using BookStore.BusinessObject.DTO.DtoForOrder;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace BookStore.MVC.Controllers
{
    public class OrderManageController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public OrderManageController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _clientFactory.CreateClient("BookStoreAPI");
            var token = HttpContext.Session.GetString("JWToken");

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var orders = await client.GetFromJsonAsync<List<OrderDTO>>("orders");

            if (orders == null)
            {
                TempData["Error"] = "Không thể tải danh sách đơn hàng.";
                return RedirectToAction("Error");
            }

            return View(orders);
        }
        /*   public async Task<IActionResult> Index()
           {
               var client = _httpClientFactory.CreateClient("BookStoreApi");
               var products = await client.GetFromJsonAsync<List<ProductDTO>>("Product");
               var categories = await client.GetFromJsonAsync<List<CategoryDTO>>("Category");
               foreach (var p in products)
               {
                   var category = categories.FirstOrDefault(c => c.CategoryId == p.CategoryId);
                   if (category != null)
                   {
                       p.CategoryName = category.CategoryName; // Đảm bảo ProductDTO có thêm thuộc tính CategoryName
                   }
               }
               retu*/


        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int orderId, int statusId)
        {
            var client = _clientFactory.CreateClient("BookStoreApi");
            var token = HttpContext.Session.GetString("JWToken");

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var content = new StringContent(statusId.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"Orders/updatestatus/{orderId}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Cập nhật trạng thái thành công.";
            }
            else
            {
                TempData["Error"] = "Lỗi cập nhật trạng thái.";
            }

            return RedirectToAction("Index");
        }
    }
}
