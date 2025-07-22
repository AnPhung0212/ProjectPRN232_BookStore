using BookStore.BusinessObject.DTO.DtoForOrder;
using BookStore.BusinessObject.Models;
using BookStore.DataAccessObject.IRepository;
using BookStore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Services.Implement
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repo;

        public OrderService(IOrderRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<OrderDTO>> GetAllAsync()
        {
            var orders = await _repo.GetAllAsync();
            return orders.Select(MapToDto);
        }

        public async Task<OrderDTO?> GetByIdAsync(int id)
        {
            var order = await _repo.GetByIdAsync(id);
            return order != null ? MapToDto(order) : null;
        }

        public async Task<int> CreateAsync(OrderCreateDTO dto)
        {
            var order = new Order
            {
                UserId = dto.UserId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = dto.Items.Sum(i => i.UnitPrice * i.Quantity),
                StatusId = 1, // default pending
                ShippingAddress = dto.ShippingAddress,
                PaymentMethod = dto.PaymentMethod,
                Phone = dto.Phone,
                OrderDetails = dto.Items.Select(i => new OrderDetail
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };
            await _repo.AddAsync(order);
            return order.OrderId;

        }

        public async Task DeleteAsync(int id) => await _repo.DeleteAsync(id);

        public async Task<IEnumerable<OrderDTO>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _repo.GetOrdersByUserIdAsync(userId);
            return orders.Select(MapToDto);
        }


        private OrderDTO MapToDto(Order o) => new OrderDTO
        {
            OrderId = o.OrderId,
            UserId = o.UserId,
            OrderDate = o.OrderDate,
            TotalAmount = o.TotalAmount,
            StatusId = o.StatusId,
            StatusName = o.Status?.StatusName,
            ShippingAddress = o.ShippingAddress,
            PaymentMethod = o.PaymentMethod,
            Phone = o.Phone, // Bổ sung dòng này
            OrderDetails = o.OrderDetails.Select(od => new OrderDetailDTO
            {
                ProductId = od.ProductId ?? 0,
                ProductTitle = od.Product?.Title,
                Quantity = od.Quantity,
                UnitPrice = od.UnitPrice,
                ImageUrl = od.Product?.ImageUrl
            }).ToList()
        };
    }
}
