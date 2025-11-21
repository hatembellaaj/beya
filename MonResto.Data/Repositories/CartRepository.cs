using Microsoft.EntityFrameworkCore;
using MonResto.Data.Context;
using MonResto.Domain.Entities;
using MonResto.Domain.Interfaces;

namespace MonResto.Data.Repositories;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _context;

    public CartRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CartItem> Add(CartItem item)
    {
        _context.CartItems.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task ClearCart(string userId)
    {
        var items = _context.CartItems.Where(ci => ci.UserId == userId);
        _context.CartItems.RemoveRange(items);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> Delete(int cartItemId)
    {
        var item = await _context.CartItems.FindAsync(cartItemId);
        if (item is null) return false;
        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<CartItem>> GetCart(string userId)
    {
        return await _context.CartItems.Include(ci => ci.Article).Where(ci => ci.UserId == userId).ToListAsync();
    }

    public async Task<CartItem?> GetItem(string userId, int articleId)
    {
        return await _context.CartItems.FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ArticleId == articleId);
    }

    public async Task<CartItem> Update(CartItem item)
    {
        _context.CartItems.Update(item);
        await _context.SaveChangesAsync();
        return item;
    }
}
