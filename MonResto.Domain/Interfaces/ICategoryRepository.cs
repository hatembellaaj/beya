using MonResto.Domain.Entities;

namespace MonResto.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAll();
    Task<Category?> GetById(int id);
    Task<Category> Add(Category entity);
    Task<Category> Update(Category entity);
    Task<bool> Delete(int id);
}
