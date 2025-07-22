namespace BookStore.MVC.ViewModels
{
    public class ProductViewModel
    {

            public int ProductId { get; set; }
            public int? CategoryId { get; set; }
            public string Title { get; set; } = null!;
            public string? Author { get; set; }
            public decimal Price { get; set; }
            public string? Description { get; set; }
            public int? Stock { get; set; }
            public string? ImageUrl { get; set; }
            public string? CategoryName { get; set; } // thêm tùy chọn

            public List<ProductViewModel> RelatedBooks { get; set; } = new List<ProductViewModel>();

    }

}

