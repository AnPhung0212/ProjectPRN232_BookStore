using BookStore.BusinessObject.DTO.DtoForOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDTO>> GetAllAsync();
        Task<OrderDTO?> GetByIdAsync(int id);
        Task CreateAsync(OrderCreateDTO dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<OrderDTO>> GetOrdersByUserIdAsync(int userId);

    }
}
