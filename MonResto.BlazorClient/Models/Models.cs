namespace MonResto.BlazorClient.Models;

public record CategoryModel(int CategoryId, string Name, string? Description);
public record ArticleModel(int ArticleId, string Name, string? Description, decimal Price, int CategoryId, string? CategoryName);
public record MenuModel(int MenuId, string Title, string? Description, IEnumerable<ArticleModel> Articles);
public record CartItemModel(int CartItemId, int ArticleId, string ArticleName, decimal Price, int Quantity);
public record OrderItemModel(int OrderItemId, int ArticleId, string ArticleName, int Quantity, decimal UnitPrice);
public record OrderModel(int OrderId, DateTime OrderDate, decimal TotalPrice, string Status, IEnumerable<OrderItemModel> Items);
public record AuthResponse(string UserName, string Token, DateTime Expires);
