namespace MonResto.Domain.Entities;

public class MenuArticle
{
    public int MenuId { get; set; }
    public Menu? Menu { get; set; }

    public int ArticleId { get; set; }
    public Article? Article { get; set; }
}
