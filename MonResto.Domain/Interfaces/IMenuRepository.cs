using MonResto.Domain.Entities;

namespace MonResto.Domain.Interfaces;

public interface IMenuRepository
{
    Task<IEnumerable<Menu>> GetAll();
    Task<Menu?> GetById(int id);
    Task<Menu> Add(Menu entity);
    Task<Menu> Update(Menu entity);
    Task<bool> Delete(int id);
    Task<bool> AddArticle(int menuId, int articleId);
    Task<bool> RemoveArticle(int menuId, int articleId);
}
