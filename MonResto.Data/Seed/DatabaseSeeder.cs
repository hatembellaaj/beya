using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MonResto.Data.Context;
using MonResto.Domain.Entities;

namespace MonResto.Data.Seed;

public static class DatabaseSeeder
{
    public const string AdminEmail = "admin@monresto.com";
    public const string AdminPassword = "Passw0rd!";
    public const string AdminRole = "Admin";

    public static async Task SeedAsync(AppDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        await context.Database.MigrateAsync();

        await EnsureAdminUserAsync(userManager, roleManager);
        await EnsureCatalogAsync(context);
    }

    private static async Task EnsureAdminUserAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync(AdminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(AdminRole));
        }

        var admin = await userManager.FindByEmailAsync(AdminEmail);
        if (admin is null)
        {
            admin = new IdentityUser
            {
                UserName = AdminEmail,
                Email = AdminEmail,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(admin, AdminPassword);
        }

        if (!await userManager.IsInRoleAsync(admin, AdminRole))
        {
            await userManager.AddToRoleAsync(admin, AdminRole);
        }
    }

    private static async Task EnsureCatalogAsync(AppDbContext context)
    {
        if (await context.Categories.AnyAsync())
        {
            return;
        }

        var pizzas = new Category
        {
            Name = "Pizzas",
            Articles = new List<Article>
            {
                new() { Name = "Margherita", Description = "Tomates, mozzarella, basilic frais", Price = 8.50m },
                new() { Name = "Reine", Description = "Jambon, champignons, mozzarella", Price = 10.00m },
                new() { Name = "4 Fromages", Description = "Mozzarella, gorgonzola, parmesan, chèvre", Price = 11.50m }
            }
        };

        var burgers = new Category
        {
            Name = "Burgers",
            Articles = new List<Article>
            {
                new() { Name = "Classic Burger", Description = "Bœuf, cheddar, salade, tomates, sauce maison", Price = 9.50m },
                new() { Name = "BBQ Burger", Description = "Bœuf, cheddar, oignons caramélisés, sauce BBQ", Price = 10.50m }
            }
        };

        var desserts = new Category
        {
            Name = "Desserts",
            Articles = new List<Article>
            {
                new() { Name = "Tiramisu", Description = "Classique italien au café", Price = 5.00m },
                new() { Name = "Fondant au chocolat", Description = "Cœur coulant, crème anglaise", Price = 5.50m }
            }
        };

        await context.Categories.AddRangeAsync(pizzas, burgers, desserts);
        await context.SaveChangesAsync();

        var menuGourmand = new Menu
        {
            Title = "Menu Gourmand",
            Description = "Entrée + plat + dessert",
            MenuArticles =
            {
                new() { ArticleId = pizzas.Articles.First().ArticleId },
                new() { ArticleId = burgers.Articles.First().ArticleId },
                new() { ArticleId = desserts.Articles.First().ArticleId }
            }
        };

        await context.Menus.AddAsync(menuGourmand);
        await context.SaveChangesAsync();
    }
}
