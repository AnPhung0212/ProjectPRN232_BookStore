using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BusinessObject.DTO
{
    public class CartItemDTO
    {
        public int CartItemId { get; set; }
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public DateTime? AddedDate { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? ImageUrl { get; set; }
    }
}
