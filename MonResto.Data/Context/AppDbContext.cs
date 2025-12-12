using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MonResto.Domain.Entities;

namespace MonResto.Data.Context;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<MenuArticle> MenuArticles => Set<MenuArticle>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<MenuArticle>().HasKey(ma => new { ma.MenuId, ma.ArticleId });

        builder.Entity<MenuArticle>()
            .HasOne(ma => ma.Menu)
            .WithMany(m => m.MenuArticles)
            .HasForeignKey(ma => ma.MenuId);

        builder.Entity<MenuArticle>()
            .HasOne(ma => ma.Article)
            .WithMany(a => a.MenuArticles)
            .HasForeignKey(ma => ma.ArticleId);

        builder.Entity<Article>()
            .HasOne(a => a.Category)
            .WithMany(c => c.Articles)
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CartItem>()
            .HasOne(ci => ci.Article)
            .WithMany(a => a.CartItems)
            .HasForeignKey(ci => ci.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<OrderItem>()
            .HasOne(oi => oi.Article)
            .WithMany(a => a.OrderItems)
            .HasForeignKey(oi => oi.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<OrderItem>()
            .HasOne(oi => oi.Order!)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Article>()
            .Property(a => a.Price)
            .HasPrecision(18, 2);

        builder.Entity<Category>()
            .Property(c => c.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Entity<Menu>()
            .Property(m => m.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice)
            .HasPrecision(18, 2);

        builder.Entity<Order>()
            .HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order!)
            .HasForeignKey(oi => oi.OrderId);

        builder.Entity<Order>()
            .Property(o => o.TotalPrice)
            .HasPrecision(18, 2);
    }
}
