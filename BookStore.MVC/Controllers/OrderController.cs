using BookStore.BusinessObject.DTO;
using BookStore.BusinessObject.DTO.DtoForOrder;
using BookStore.BusinessObject.DTO.UserDTOs;
using BookStore.BusinessObject.Models;
using BookStore.MVC.Helpers;
using BookStore.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace BookStore.MVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public OrderController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(string ShippingAddress, string PhoneNumber, string PaymentMethod)
        {
            // Kiểm tra login
            var token = HttpContext.Session.GetString("JWToken");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) // thêm log
            {
                Console.WriteLine("Không tìm thấy UserId trong session");
            }

            if (string.IsNullOrWhiteSpace(ShippingAddress) ||string.IsNullOrWhiteSpace(PhoneNumber) ||string.IsNullOrWhiteSpace(PaymentMethod))
            {
                TempData["Error"] = "Vui lòng điền đầy đủ thông tin giao hàng và chọn phương thức thanh toán.";
                return RedirectToAction("ViewCart", "Cart");
            }
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userIdStr))
            {
                // Chưa login → chuyển hướng đến Login
                return RedirectToAction("Login", "Account");
            }

            if (PaymentMethod == "BankTransfer")
            {
                TempData["Error"] = "Chức năng chuyển khoản ngân hàng chưa được hỗ trợ.";
                return RedirectToAction("ViewCart", "Cart");
            }

            // Lấy cart từ session
            var cart = HttpContext.Session.GetObjectFromJson<CartDTOForSession>("CART_SESSION");
            if (cart == null || !cart.Items.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("ViewCart", "Cart");
            }
            int userId = int.Parse(userIdStr);

            // Tạo OrderCreateDTO
            var orderDto = new OrderCreateDTO
            {
                UserId = userId,
                ShippingAddress = ShippingAddress,
                Phone = PhoneNumber,
                PaymentMethod = PaymentMethod,
                Items = cart.Items.Select(item => new OrderItemCreateDTO
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice ?? 0
                }).ToList()
            };

            // Gọi API để lấy UserId theo email
            var client = _clientFactory.CreateClient("BookStoreApi");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Gửi order lên API
            var orderResponse = await client.PostAsJsonAsync("orders/create", orderDto);
            if (!orderResponse.IsSuccessStatusCode)
            {
                TempData["Error"] = "Đặt hàng thất bại. Vui lòng thử lại.";
                return RedirectToAction("ViewCart", "Cart");
            }

            // Đọc orderId từ phản hồi
            int orderId = await orderResponse.Content.ReadFromJsonAsync<int>();

            // Xóa giỏ hàng sau khi đặt hàng thành công
            HttpContext.Session.Remove("CART_SESSION");

            // Chuyển đến trang hiển thị hóa đơn
            TempData["Success"] = "Đặt hàng thành công!";
            return RedirectToAction("Invoice", new { id = orderId });
        }

        [HttpGet]
        public async Task<IActionResult> Invoice(int id)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _clientFactory.CreateClient("BookStoreApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Gọi API lấy đơn hàng theo id
            var response = await client.GetAsync($"orders/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound(); // hoặc trả lỗi rõ ràng hơn
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var order = JsonConvert.DeserializeObject<OrderDTO>(responseBody);

            if (order == null)
            {
                return NotFound();
            }

            ViewBag.SuccessMessage = TempData["Success"];
            return View(order); // Truyền đúng model OrderDTO vào view
        }

    }
}
