using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MonResto.Domain.Entities;

public class CartItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CartItemId { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public int ArticleId { get; set; }
    public Article? Article { get; set; }

    public int Quantity { get; set; }
}
