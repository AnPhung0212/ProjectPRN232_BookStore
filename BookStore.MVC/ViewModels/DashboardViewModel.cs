namespace BookStore.MVC.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public int TotalBooks { get; set; }
        public int TotalCategories { get; set; }
        public int TotalAuthors { get; set; }
        public int TotalBooksSold { get; set; }
        public int TotalStock { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<CategoryProductStat> ProductPerCategory { get; set; } = new();
        public List<OrderPerDayStat> OrdersPerDay { get; set; } = new();
        public List<RevenuePerDayStat> RevenuePerDay { get; set; } = new();
        public List<OrderStatusStat> OrderStatusStats { get; set; } = new();
        public List<TopProductStat> TopProducts { get; set; } = new();
        public Dictionary<string, int> CategoryBookCounts { get; set; } = new();
        public Dictionary<string, int> CategorySalesCounts { get; set; } = new();
    }

    public class CategoryProductStat
    {
        public string CategoryName { get; set; }
        public int ProductCount { get; set; }
    }

    public class OrderPerDayStat
    {
        public string Date { get; set; } // ví dụ: "2025-07-20"
        public int OrderCount { get; set; }
    }

    public class RevenuePerDayStat
    {
        public string Date { get; set; }
        public decimal Revenue { get; set; }
    }

    public class OrderStatusStat
    {
        public string Status { get; set; } // ví dụ: "Pending", "Delivered", "Cancelled"
        public int Count { get; set; }
    }

    public class TopProductStat
    {
        public string ProductName { get; set; }
        public int QuantitySold { get; set; }
    }
}