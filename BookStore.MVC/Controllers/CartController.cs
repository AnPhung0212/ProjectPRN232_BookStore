using BookStore.BusinessObject.DTO;
using BookStore.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BookStore.MVC.Controllers
{
    public class CartController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CartController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private const string CART_KEY = "CART_SESSION";

        private CartDTOForSession GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString(CART_KEY);
            if (string.IsNullOrEmpty(cartJson)) return new CartDTOForSession();
            return JsonConvert.DeserializeObject<CartDTOForSession>(cartJson) ?? new CartDTOForSession();
        }

        private void SaveCartToSession(CartDTOForSession cart)
        {
            var cartJson = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString(CART_KEY, cartJson);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId)
        {
            var cart = GetCartFromSession();

            var existingItem = cart.Items.FirstOrDefault(p => p.ProductId == productId);
            if (existingItem != null)
            {
                if (existingItem.Quantity < existingItem.Stock)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    TempData["Error"] = "Số lượng đã đạt tối đa tồn kho.";
                }
            }
            else
            {
                var client = _httpClientFactory.CreateClient("BookStoreAPI");
                var response = await client.GetAsync($"product/{productId}");

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Không thể lấy thông tin sản phẩm.";
                    return RedirectToAction("Index", "ProductMVC");
                }

                var product = await response.Content.ReadFromJsonAsync<ProductDTO>();
                if (product == null)
                {
                    TempData["Error"] = "Sản phẩm không tồn tại.";
                    return RedirectToAction("Index", "ProductMVC");
                }

                cart.Items.Add(new CartItemDTOForSession
                {
                    ProductId = product.ProductId,
                    ProductName = product.Title,
                    Quantity = 1,
                    UnitPrice = product.Price,
                    ImageUrl = product.ImageUrl,
                    Stock = (int)(product.Stock ?? 0)
                });
            }

            SaveCartToSession(cart);
            TempData["Message"] = "Đã thêm sản phẩm vào giỏ hàng!";
            return RedirectToAction("Index", "ProductMVC");
        }

        public IActionResult ViewCart()
        {
            var cart = GetCartFromSession();
            return View(cart);
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity, string actionType)
        {
            var cart = GetCartFromSession();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm trong giỏ.";
                return RedirectToAction("ViewCart");
            }

            int stock = item.Stock;

            if (actionType == "increase")
            {
                if (item.Quantity < stock)
                {
                    item.Quantity++;
                }
                else
                {
                    TempData["Error"] = "Số lượng đã đạt tối đa tồn kho.";
                }
            }
            else if (actionType == "decrease")
            {
                if (item.Quantity > 1)
                {
                    item.Quantity--;
                }
            }
            else if (actionType == "direct")
            {
                if (quantity < 1)
                {
                    item.Quantity = 1;
                }
                else if (quantity > stock)
                {
                    item.Quantity = stock;
                    TempData["Error"] = $"Số lượng vượt quá tồn kho. Đã đặt lại = {stock}.";
                }
                else
                {
                    item.Quantity = quantity;
                }
            }

            SaveCartToSession(cart);
            return RedirectToAction("ViewCart");
        }


        [HttpPost]
        public IActionResult RemoveItem(int productId)
        {
            var cart = GetCartFromSession();

            cart.Items.RemoveAll(i => i.ProductId == productId);

            SaveCartToSession(cart);
            return RedirectToAction("ViewCart");
        }
    }
}
