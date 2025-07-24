using BookStore.BusinessObject.DTO;

namespace BookStore.MVC.ViewModels
{
    public class ProductCreateViewModel
    {
        public ProductCreateDTO Product { get; set; } = new ProductCreateDTO();
        public List<CategoryDTO> Categories { get; set; } = new List<CategoryDTO>();
        public UpdateProductDTO ProductEdit { get; set; } = new UpdateProductDTO(); // cho Edit

    }
}
