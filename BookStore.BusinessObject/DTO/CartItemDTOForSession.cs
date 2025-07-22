using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BusinessObject.DTO
{
    public class CartItemDTOForSession
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal? UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }
        public int Stock { get; set; } // Số lượng tồn kho

    }
}
