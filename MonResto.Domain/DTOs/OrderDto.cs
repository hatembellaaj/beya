using System.Collections.Generic;
using System.Linq;
using MonResto.Domain.Enums;

namespace MonResto.Domain.DTOs;

public class OrderItemDto
{
    public int OrderItemId { get; set; }
    public int ArticleId { get; set; }
    public string ArticleName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class OrderDto
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public OrderStatus Status { get; set; }
    public IEnumerable<OrderItemDto> Items { get; set; } = Enumerable.Empty<OrderItemDto>();
}

public record CreateOrderDto(IEnumerable<CartItemCreateDto> Items);
