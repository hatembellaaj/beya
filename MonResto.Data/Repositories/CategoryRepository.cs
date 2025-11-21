using Microsoft.EntityFrameworkCore;
using MonResto.Data.Context;
using MonResto.Domain.Entities;
using MonResto.Domain.Interfaces;

namespace MonResto.Data.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Category> Add(Category entity)
    {
        _context.Categories.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> Delete(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category is null) return false;
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Category>> GetAll()
    {
        return await _context.Categories.AsNoTracking().Include(c => c.Articles).ToListAsync();
    }

    public async Task<Category?> GetById(int id)
    {
        return await _context.Categories.Include(c => c.Articles).FirstOrDefaultAsync(c => c.CategoryId == id);
    }

    public async Task<Category> Update(Category entity)
    {
        _context.Categories.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}
