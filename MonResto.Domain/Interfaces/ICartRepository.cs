using MonResto.Domain.Entities;

namespace MonResto.Domain.Interfaces;

public interface ICartRepository
{
    Task<IEnumerable<CartItem>> GetCart(string userId);
    Task<CartItem?> GetItem(string userId, int articleId);
    Task<CartItem> Add(CartItem item);
    Task<CartItem> Update(CartItem item);
    Task<bool> Delete(int cartItemId);
    Task ClearCart(string userId);
}
