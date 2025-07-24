using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BusinessObject.DTO
{
    public class ProductCreateDTO
    {
        
            [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
            public string Title { get; set; } = null!;

            public string? Author { get; set; }

            [Required(ErrorMessage = "Giá là bắt buộc")]
            [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
            public decimal Price { get; set; }

            public string? Description { get; set; }

            [Required(ErrorMessage = "Số lượng tồn là bắt buộc")]
            [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn không hợp lệ")]
            public int? Stock { get; set; }

            public string? ImageUrl { get; set; }

            [Required(ErrorMessage = "Vui lòng chọn danh mục")]
            public int? CategoryId { get; set; }



    }
}
