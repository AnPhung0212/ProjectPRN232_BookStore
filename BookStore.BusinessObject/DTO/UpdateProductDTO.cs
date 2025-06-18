using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BusinessObject.DTO
{
    public class UpdateProductDTO
    {
        public int ProductId { get; set; } // dùng để xác định bản ghi

        public int? CategoryId { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public int? Stock { get; set; }
        public string? ImageUrl { get; set; }
    }
}
