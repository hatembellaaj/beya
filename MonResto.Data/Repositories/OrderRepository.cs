using Microsoft.EntityFrameworkCore;
using MonResto.Data.Context;
using MonResto.Domain.Entities;
using MonResto.Domain.Enums;
using MonResto.Domain.Interfaces;

namespace MonResto.Data.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order> Add(Order entity)
    {
        _context.Orders.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<IEnumerable<Order>> GetAll()
    {
        return await _context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Article).ToListAsync();
    }

    public async Task<Order?> GetById(int id)
    {
        return await _context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Article)
            .FirstOrDefaultAsync(o => o.OrderId == id);
    }

    public async Task<IEnumerable<Order>> GetByUser(string userId)
    {
        return await _context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Article)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<Order> UpdateStatus(int orderId, OrderStatus status)
    {
        var order = await _context.Orders.FindAsync(orderId) ?? throw new InvalidOperationException("Order not found");
        order.Status = status;
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        return order;
    }
}
