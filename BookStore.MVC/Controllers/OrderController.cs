using BookStore.BusinessObject.DTO;
using BookStore.BusinessObject.DTO.DtoForOrder;
using BookStore.MVC.Helpers;
using BookStore.MVC.Library;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using VNPAY.NET.Models;

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
            var token = HttpContext.Session.GetString("JWToken");
            var userIdStr = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrWhiteSpace(ShippingAddress) || string.IsNullOrWhiteSpace(PhoneNumber) || string.IsNullOrWhiteSpace(PaymentMethod))
            {
                TempData["Error"] = "Vui lòng điền đầy đủ thông tin giao hàng và chọn phương thức thanh toán.";
                return RedirectToAction("ViewCart", "Cart");
            }

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = HttpContext.Session.GetObjectFromJson<CartDTOForSession>("CART_SESSION");
            if (cart == null || !cart.Items.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("ViewCart", "Cart");
            }

            int userId = int.Parse(userIdStr);

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

            if (PaymentMethod == "VNPAY")
            {
                var totalAmount = cart.Items.Sum(i => i.Quantity * (i.UnitPrice ?? 0)) + 30000;
                HttpContext.Session.SetObjectAsJson("PENDING_ORDER", orderDto);

                var paymentRequest = new PaymentRequest
                {
                    PaymentId = 0,
                    Money = (double)totalAmount
                };

                var vnpayService = new VnpayService(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build());
                string paymentUrl = vnpayService.GetPaymentUrl(paymentRequest);

                return Redirect(paymentUrl);
            }

            var client = _clientFactory.CreateClient("BookStoreAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var orderResponse = await client.PostAsJsonAsync("Orders/create", orderDto);
            if (!orderResponse.IsSuccessStatusCode)
            {
                TempData["Error"] = "Đặt hàng thất bại. Vui lòng thử lại.";
                return RedirectToAction("ViewCart", "Cart");
            }

            int orderId = await orderResponse.Content.ReadFromJsonAsync<int>();
            HttpContext.Session.Remove("CART_SESSION");

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

            var client = _clientFactory.CreateClient("BookStoreAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"Orders/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var order = JsonConvert.DeserializeObject<OrderDTO>(responseBody);

            if (order == null)
            {
                return NotFound();
            }

            ViewBag.SuccessMessage = TempData["Success"];
            return View(order);
        }

        public async Task<IActionResult> OrderHistory()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdStr);

            ViewBag.UserId = userId;

            var client = _clientFactory.CreateClient("BookStoreAPI");

            var token = HttpContext.Session.GetString("JWToken");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"Orders/user/{userId}");
            Console.WriteLine($"[OrderHistory] Status: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var orders = System.Text.Json.JsonSerializer.Deserialize<List<OrderDTO>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return View(orders);
            }
            else
            {
                ViewBag.Error = "Không thể lấy lịch sử đơn hàng.";
                return View(new List<OrderDTO>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> PaymentReturn()
        {
            var vnpayService = new VnpayService(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build());

            var response = vnpayService.GetPaymentResult(Request.Query);
            if (!response.IsSuccess)
            {
                TempData["Error"] = "Thanh toán thất bại hoặc bị hủy.";
                return RedirectToAction("ViewCart", "Cart");
            }

            var orderDto = HttpContext.Session.GetObjectFromJson<OrderCreateDTO>("PENDING_ORDER");
            if (orderDto == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin đơn hàng.";
                return RedirectToAction("ViewCart", "Cart");
            }

            var token = HttpContext.Session.GetString("JWToken");
            var client = _clientFactory.CreateClient("BookStoreAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var orderResponse = await client.PostAsJsonAsync("Orders/create", orderDto);
            if (!orderResponse.IsSuccessStatusCode)
            {
                TempData["Error"] = "Tạo đơn hàng thất bại sau khi thanh toán.";
                return RedirectToAction("ViewCart", "Cart");
            }

            int orderId = await orderResponse.Content.ReadFromJsonAsync<int>();

            HttpContext.Session.Remove("CART_SESSION");
            HttpContext.Session.Remove("PENDING_ORDER");

            TempData["Success"] = "Thanh toán và đặt hàng thành công!";
            return RedirectToAction("Invoice", new { id = orderId });
        }

    }
}
