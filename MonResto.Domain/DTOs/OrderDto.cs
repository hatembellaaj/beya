using System.Collections.Generic;
using MonResto.Domain.Enums;

namespace MonResto.Domain.DTOs;

public record OrderItemDto(int OrderItemId, int ArticleId, string ArticleName, int Quantity, decimal UnitPrice);
public record OrderDto(int OrderId, DateTime OrderDate, decimal TotalPrice, OrderStatus Status, IEnumerable<OrderItemDto> Items);
public record CreateOrderDto(IEnumerable<CartItemCreateDto> Items);
