namespace MonResto.Domain.DTOs;

public record CartItemDto(int CartItemId, int ArticleId, string ArticleName, decimal Price, int Quantity);
public record CartItemCreateDto(int ArticleId, int Quantity);
public record CartSummaryDto(int TotalQuantity, decimal TotalPrice);
