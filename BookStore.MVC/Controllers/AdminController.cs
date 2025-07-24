using BookStore.BusinessObject.DTO.DtoForOrder;
using BookStore.BusinessObject.DTO;
using BookStore.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BookStore.MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("BookStoreApi");
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var dashboard = new DashboardViewModel();

            // Lấy dữ liệu từ API
            var products = await client.GetFromJsonAsync<List<ProductDTO>>("Product");
            var categories = await client.GetFromJsonAsync<List<CategoryDTO>>("Category");
            var orderResponse = await client.GetAsync("Orders");
            List<OrderDTO>? orders = null;
            if (orderResponse.IsSuccessStatusCode)
            {
                orders = await orderResponse.Content.ReadFromJsonAsync<List<OrderDTO>>();
            }

            // Tổng quan
            dashboard.TotalProducts = products?.Count ?? 0;
            dashboard.TotalBooks = products?.Count ?? 0; // Giả sử TotalBooks = TotalProducts
            dashboard.TotalCategories = categories?.Count ?? 0;
            dashboard.TotalOrders = orders?.Count ?? 0;
            dashboard.TotalAuthors = products?.Select(p => p.Author).Distinct().Count() ?? 0; // Giả sử ProductDTO có Author
            dashboard.TotalBooksSold = orders?.SelectMany(o => o.OrderDetails).Sum(od => od.Quantity) ?? 0;
            dashboard.TotalStock = products?.Sum(p => p.Stock) ?? 0; // Giả sử ProductDTO có Stock
            dashboard.TotalRevenue = orders?.Sum(o => o.TotalAmount ?? 0) ?? 0;

            // Sản phẩm theo danh mục
            dashboard.ProductPerCategory = categories?.Select(c => new CategoryProductStat
            {
                CategoryName = c.CategoryName,
                ProductCount = products?.Count(p => p.CategoryId == c.CategoryId) ?? 0
            }).ToList() ?? new List<CategoryProductStat>();

            // CategoryBookCounts
            dashboard.CategoryBookCounts = categories?.ToDictionary(
                c => c.CategoryName,
                c => products?.Count(p => p.CategoryId == c.CategoryId) ?? 0
            ) ?? new Dictionary<string, int>();

            // CategorySalesCounts
            dashboard.CategorySalesCounts = orders?
                .SelectMany(o => o.OrderDetails)
                .GroupBy(od => products?.FirstOrDefault(p => p.ProductId == od.ProductId)?.CategoryId)
                .ToDictionary(
                    g => categories?.FirstOrDefault(c => c.CategoryId == g.Key)?.CategoryName ?? "Unknown",
                    g => g.Sum(od => od.Quantity)
                ) ?? new Dictionary<string, int>();

            // Đơn hàng theo ngày
            dashboard.OrdersPerDay = orders?
                .GroupBy(o => o.OrderDate)
                .Select(g => new OrderPerDayStat
                {
                    Date = g.Key?.ToString("yyyy-MM-dd"),
                    OrderCount = g.Count()
                })
                .OrderBy(o => o.Date)
                .ToList() ?? new List<OrderPerDayStat>();

            // Doanh thu theo ngày
            dashboard.RevenuePerDay = orders?
                .GroupBy(o => o.OrderDate)
                .Select(g => new RevenuePerDayStat
                {
                    Date = g.Key?.ToString("yyyy-MM-dd"),
                    Revenue = g.Sum(o => o.TotalAmount ?? 0)
                })
                .OrderBy(r => r.Date)
                .ToList() ?? new List<RevenuePerDayStat>();

            // Trạng thái đơn hàng
            dashboard.OrderStatusStats = orders?
                .GroupBy(o => o.StatusName)
                .Select(g => new OrderStatusStat
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToList() ?? new List<OrderStatusStat>();

            // Top 5 sản phẩm bán chạy
            dashboard.TopProducts = orders?
                .SelectMany(o => o.OrderDetails)
                .GroupBy(od => od.ProductId)
                .Select(g => new TopProductStat
                {
                    ProductName = products?.FirstOrDefault(p => p.ProductId == g.Key)?.Title ?? "Unknown",
                    QuantitySold = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(p => p.QuantitySold)
                .Take(5)
                .ToList() ?? new List<TopProductStat>();

            return View(dashboard);
        }
    }
}
