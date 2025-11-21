namespace MonResto.Domain.DTOs;

public record ArticleDto(int ArticleId, string Name, string? Description, decimal Price, int CategoryId, string? CategoryName);
public record ArticleCreateDto(string Name, string? Description, decimal Price, int CategoryId);
