namespace BookStore.MVC.ViewModels
{
    public class ProductListViewModel
    {
        public List<ProductViewModel> Products { get; set; }
        public List<CategoryViewModel> Categories { get; set; }
        public string? SearchKeyword { get; set; }
        public int? SelectedCategoryId { get; set; }
    }
}
