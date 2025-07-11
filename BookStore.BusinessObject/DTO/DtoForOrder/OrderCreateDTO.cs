using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BusinessObject.DTO.DtoForOrder
{
    public class OrderCreateDTO
    {
        public int UserId { get; set; }
        public string ShippingAddress { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public List<OrderItemCreateDTO> Items { get; set; } = new();
    }

    public class OrderItemCreateDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
