using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MonResto.Domain.Entities;

public class Menu
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MenuId { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(800)]
    public string? Description { get; set; }

    public ICollection<MenuArticle> MenuArticles { get; set; } = new List<MenuArticle>();
}
