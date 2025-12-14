namespace MonResto.Domain.DTOs;

public record CartItemDto
{
    public CartItemDto() { }

    public int CartItemId { get; init; }
    public int ArticleId { get; init; }
    public string ArticleName { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Quantity { get; init; }
}

public record CartItemCreateDto(int ArticleId, int Quantity);
public record CartSummaryDto(int TotalQuantity, decimal TotalPrice);
