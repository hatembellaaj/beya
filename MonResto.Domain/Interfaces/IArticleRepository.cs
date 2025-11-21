using MonResto.Domain.Entities;

namespace MonResto.Domain.Interfaces;

public interface IArticleRepository
{
    Task<IEnumerable<Article>> GetAll();
    Task<IEnumerable<Article>> GetByCategory(int categoryId);
    Task<IEnumerable<Article>> SearchByName(string name);
    Task<Article?> GetById(int id);
    Task<Article> Add(Article entity);
    Task<Article> Update(Article entity);
    Task<bool> Delete(int id);
}
