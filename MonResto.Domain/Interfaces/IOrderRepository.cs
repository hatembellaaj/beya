using MonResto.Domain.Entities;
using MonResto.Domain.Enums;

namespace MonResto.Domain.Interfaces;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAll();
    Task<IEnumerable<Order>> GetByUser(string userId);
    Task<Order?> GetById(int id);
    Task<Order> Add(Order entity);
    Task<Order> UpdateStatus(int orderId, OrderStatus status);
}
