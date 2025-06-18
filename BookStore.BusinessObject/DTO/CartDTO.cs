using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BusinessObject.DTO
{
    public class CartDTO
    {
        public int CartId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<CartItemDTO> Items { get; set; } = new();
    }
}
