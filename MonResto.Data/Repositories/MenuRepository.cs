using Microsoft.EntityFrameworkCore;
using MonResto.Data.Context;
using MonResto.Domain.Entities;
using MonResto.Domain.Interfaces;

namespace MonResto.Data.Repositories;

public class MenuRepository : IMenuRepository
{
    private readonly AppDbContext _context;

    public MenuRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Menu> Add(Menu entity)
    {
        _context.Menus.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> AddArticle(int menuId, int articleId)
    {
        var existing = await _context.MenuArticles.FindAsync(menuId, articleId);
        if (existing != null) return true;
        _context.MenuArticles.Add(new MenuArticle { MenuId = menuId, ArticleId = articleId });
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> Delete(int id)
    {
        var menu = await _context.Menus.FindAsync(id);
        if (menu is null) return false;
        _context.Menus.Remove(menu);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Menu>> GetAll()
    {
        return await _context.Menus
            .Include(m => m.MenuArticles)
            .ThenInclude(ma => ma.Article)
            .ThenInclude(a => a!.Category)
            .ToListAsync();
    }

    public async Task<Menu?> GetById(int id)
    {
        return await _context.Menus
            .Include(m => m.MenuArticles)
            .ThenInclude(ma => ma.Article)
            .ThenInclude(a => a!.Category)
            .FirstOrDefaultAsync(m => m.MenuId == id);
    }

    public async Task<bool> RemoveArticle(int menuId, int articleId)
    {
        var link = await _context.MenuArticles.FindAsync(menuId, articleId);
        if (link is null) return false;
        _context.MenuArticles.Remove(link);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Menu> Update(Menu entity)
    {
        _context.Menus.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}
