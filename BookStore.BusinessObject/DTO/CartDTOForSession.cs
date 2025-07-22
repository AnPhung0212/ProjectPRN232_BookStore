using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BusinessObject.DTO
{
    public class CartDTOForSession
    {
        public List<CartItemDTOForSession> Items { get; set; } = new();

    }
}
