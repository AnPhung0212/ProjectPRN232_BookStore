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
        private readonly IOrderRepository _repo;
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository repo, IProductRepository productRepository)
        {
            _repo = repo;
            _productRepository = productRepository;
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
                OrderDetails = new List<OrderDetail>()
            };

            // Duyệt qua từng item và kiểm tra tồn kho
            foreach (var item in dto.Items)
            {
                var product = await _productRepository.GetProductByIdAsync(item.ProductId);
                if (product == null)
                {
                    throw new Exception($"Không tìm thấy sản phẩm có ID {item.ProductId}.");
                }

                if (product.Stock == null || product.Stock < item.Quantity)
                {
                    throw new Exception($"Sản phẩm '{product.Title}' không đủ hàng. Tồn kho: {product.Stock}, yêu cầu: {item.Quantity}.");
                }

                // Trừ số lượng tồn kho
                product.Stock -= item.Quantity;
                await _productRepository.UpdateProductAsync(product);

                // Thêm vào chi tiết đơn hàng
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
            }

            // Thêm đơn hàng vào DB
            await _repo.AddAsync(order);

            return order.OrderId;
        }

        public async Task DeleteAsync(int id) => await _repo.DeleteAsync(id);

        public async Task<IEnumerable<OrderDTO>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _repo.GetOrdersByUserIdAsync(userId);
            return orders.Select(MapToDto);
        }
        public async Task<bool> UpdateStatusAsync(int orderId, int statusId)
        {
            var order = await _repo.GetByIdAsync(orderId);
            if (order == null) return false;

            order.StatusId = statusId;
            await _repo.UpdateAsync(order);
            return true;
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
