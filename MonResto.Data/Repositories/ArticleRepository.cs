using Microsoft.EntityFrameworkCore;
using MonResto.Data.Context;
using MonResto.Domain.Entities;
using MonResto.Domain.Interfaces;

namespace MonResto.Data.Repositories;

public class ArticleRepository : IArticleRepository
{
    private readonly AppDbContext _context;

    public ArticleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Article> Add(Article entity)
    {
        _context.Articles.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> Delete(int id)
    {
        var article = await _context.Articles.FindAsync(id);
        if (article is null) return false;
        _context.Articles.Remove(article);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Article>> GetAll()
    {
        return await _context.Articles.Include(a => a.Category).AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Article>> GetByCategory(int categoryId)
    {
        return await _context.Articles.Include(a => a.Category).Where(a => a.CategoryId == categoryId).ToListAsync();
    }

    public async Task<Article?> GetById(int id)
    {
        return await _context.Articles.Include(a => a.Category).FirstOrDefaultAsync(a => a.ArticleId == id);
    }

    public async Task<IEnumerable<Article>> SearchByName(string name)
    {
        return await _context.Articles.Include(a => a.Category)
            .Where(a => a.Name.ToLower().Contains(name.ToLower()))
            .ToListAsync();
    }

    public async Task<Article> Update(Article entity)
    {
        _context.Articles.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}
