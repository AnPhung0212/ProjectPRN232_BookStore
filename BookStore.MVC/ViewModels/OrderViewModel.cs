namespace BookStore.MVC.ViewModels
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? StatusName { get; set; }
        public string? ShippingAddress { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Phone { get; set; }

        public List<OrderDetailViewModel> OrderDetails { get; set; } = new();
    }
    public class OrderDetailViewModel
    {
        public int ProductId { get; set; }
        public string? ProductTitle { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
