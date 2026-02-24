using BookStore.BusinessObject.DTO.DtoForOrder;
using BookStore.BusinessObject.Models;
using BookStore.DataAccessObject.IRepository;
using BookStore.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Services.Implement
{
    public class OrderService : IOrderService
    {
        private readonly IGenericRepository<Order> _orderRepo;
        private readonly IGenericRepository<Product> _productRepo;

        public OrderService(IGenericRepository<Order> orderRepo, IGenericRepository<Product> productRepo)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
        }

        public async Task<IEnumerable<OrderDTO>> GetAllAsync()
        {
            var orders = await _orderRepo.Entities
                .Include(o => o.OrderDetails).ThenInclude(d => d.Product)
                .Include(o => o.Status)
                .Include(o => o.User)
                .ToListAsync();
            return orders.Select(MapToDto);
        }

        public async Task<OrderDTO?> GetByIdAsync(int id)
        {
            var order = await _orderRepo.Entities
                .Include(o => o.OrderDetails).ThenInclude(d => d.Product)
                .Include(o => o.Status)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderID == id);
            return order != null ? MapToDto(order) : null;
        }

        public async Task<int> CreateAsync(OrderCreateDTO dto)
        {
            var order = new Order
            {
                UserID = dto.UserId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = dto.Items.Sum(i => i.UnitPrice * i.Quantity),
                StatusID = 1, // default pending
                ShippingAddress = dto.ShippingAddress,
                PaymentMethod = dto.PaymentMethod,
                Phone = dto.Phone,
                OrderDetails = new List<OrderDetail>()
            };

            foreach (var item in dto.Items)
            {
                var product = await _productRepo.GetByIdAsync(item.ProductId);
                if (product == null)
                {
                    throw new Exception($"Không tìm thấy sản phẩm có ID {item.ProductId}.");
                }

                if (product.Stock == null || product.Stock < item.Quantity)
                {
                    throw new Exception($"Sản phẩm '{product.Title}' không đủ hàng. Tồn kho: {product.Stock}, yêu cầu: {item.Quantity}.");
                }

                product.Stock -= item.Quantity;
                await _productRepo.UpdateAsync(product);

                order.OrderDetails.Add(new OrderDetail
                {
                    ProductID = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
            }

            await _orderRepo.AddAsync(order);

            return order.OrderID;
        }

        public async Task DeleteAsync(int id) => await _orderRepo.DeleteAsync(id);

        public async Task<IEnumerable<OrderDTO>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _orderRepo.Entities
                .Include(o => o.OrderDetails).ThenInclude(d => d.Product)
                .Include(o => o.Status)
                .Include(o => o.User)
                .Where(o => o.UserID == userId)
                .ToListAsync();
            return orders.Select(MapToDto);
        }

        public async Task<bool> UpdateStatusAsync(int orderId, int statusId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null) return false;

            order.StatusID = statusId;
            await _orderRepo.UpdateAsync(order);
            return true;
        }

        private OrderDTO MapToDto(Order o) => new OrderDTO
        {
            OrderId = o.OrderID,
            UserId = o.UserID,
            OrderDate = o.OrderDate,
            TotalAmount = o.TotalAmount,
            StatusId = o.StatusID,
            StatusName = o.Status?.StatusName,
            ShippingAddress = o.ShippingAddress,
            PaymentMethod = o.PaymentMethod,
            Phone = o.Phone,
            OrderDetails = o.OrderDetails?.Select(od => new OrderDetailDTO
            {
                ProductId = od.ProductID ?? 0,
                ProductTitle = od.Product?.Title,
                Quantity = od.Quantity,
                UnitPrice = od.UnitPrice,
                ImageUrl = od.Product?.ImageURL
            }).ToList() ?? new List<OrderDetailDTO>()
        };
    }
}
