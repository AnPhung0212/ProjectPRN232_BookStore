using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BusinessObject.DTO.DtoForOrder
{
    public class OrderWithDetailsDTO
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? StatusName { get; set; }
        public string? ShippingAddress { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Phone { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; } = new();
    }

}
