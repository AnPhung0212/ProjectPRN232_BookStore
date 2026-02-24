namespace BookStore.MVC.ViewModels
{
    public class HomePageViewModel
    {
        public List<ProductViewModel> AllProducts { get; set; } = new();
        public List<ProductViewModel> Top3ExpensiveProducts { get; set; } = new();
        public List<ProductViewModel> Top3NewestProducts { get; set; } = new();
        public List<CategoryViewModel> Categories { get; set; }
        public string SearchKeyword { get; set; }
        public int? SelectedCategoryId { get; set; }
    }
}
