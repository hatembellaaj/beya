using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonResto.Data.Migrations
{
    public partial class SeedMenusData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
DO $$
DECLARE gourmet_menu_id integer;
DECLARE duo_menu_id integer;
DECLARE margherita_id integer;
DECLARE reine_id integer;
DECLARE quatre_fromages_id integer;
DECLARE classic_burger_id integer;
DECLARE bbq_burger_id integer;
DECLARE tiramisu_id integer;
DECLARE fondant_id integer;
BEGIN
    SELECT "MenuId" INTO gourmet_menu_id FROM "Menus" WHERE "Title" = 'Menu Gourmand';
    IF gourmet_menu_id IS NULL THEN
        INSERT INTO "Menus" ("Title", "Description")
        VALUES ('Menu Gourmand', 'Entrée + plat + dessert')
        RETURNING "MenuId" INTO gourmet_menu_id;
    END IF;

    SELECT "MenuId" INTO duo_menu_id FROM "Menus" WHERE "Title" = 'Menu Duo';
    IF duo_menu_id IS NULL THEN
        INSERT INTO "Menus" ("Title", "Description")
        VALUES ('Menu Duo', 'Deux plats à partager')
        RETURNING "MenuId" INTO duo_menu_id;
    END IF;

    SELECT "ArticleId" INTO margherita_id FROM "Articles" WHERE "Name" = 'Margherita' ORDER BY "ArticleId" LIMIT 1;
    SELECT "ArticleId" INTO reine_id FROM "Articles" WHERE "Name" = 'Reine' ORDER BY "ArticleId" LIMIT 1;
    SELECT "ArticleId" INTO quatre_fromages_id FROM "Articles" WHERE "Name" = '4 Fromages' ORDER BY "ArticleId" LIMIT 1;
    SELECT "ArticleId" INTO classic_burger_id FROM "Articles" WHERE "Name" = 'Classic Burger' ORDER BY "ArticleId" LIMIT 1;
    SELECT "ArticleId" INTO bbq_burger_id FROM "Articles" WHERE "Name" = 'BBQ Burger' ORDER BY "ArticleId" LIMIT 1;
    SELECT "ArticleId" INTO tiramisu_id FROM "Articles" WHERE "Name" = 'Tiramisu' ORDER BY "ArticleId" LIMIT 1;
    SELECT "ArticleId" INTO fondant_id FROM "Articles" WHERE "Name" = 'Fondant au chocolat' ORDER BY "ArticleId" LIMIT 1;

    IF gourmet_menu_id IS NOT NULL THEN
        IF margherita_id IS NOT NULL THEN
            INSERT INTO "MenuArticles" ("MenuId", "ArticleId")
            SELECT gourmet_menu_id, margherita_id
            WHERE NOT EXISTS (
                SELECT 1 FROM "MenuArticles" WHERE "MenuId" = gourmet_menu_id AND "ArticleId" = margherita_id
            );
        END IF;

        IF classic_burger_id IS NOT NULL THEN
            INSERT INTO "MenuArticles" ("MenuId", "ArticleId")
            SELECT gourmet_menu_id, classic_burger_id
            WHERE NOT EXISTS (
                SELECT 1 FROM "MenuArticles" WHERE "MenuId" = gourmet_menu_id AND "ArticleId" = classic_burger_id
            );
        END IF;

        IF tiramisu_id IS NOT NULL THEN
            INSERT INTO "MenuArticles" ("MenuId", "ArticleId")
            SELECT gourmet_menu_id, tiramisu_id
            WHERE NOT EXISTS (
                SELECT 1 FROM "MenuArticles" WHERE "MenuId" = gourmet_menu_id AND "ArticleId" = tiramisu_id
            );
        END IF;
    END IF;

    IF duo_menu_id IS NOT NULL THEN
        IF reine_id IS NOT NULL THEN
            INSERT INTO "MenuArticles" ("MenuId", "ArticleId")
            SELECT duo_menu_id, reine_id
            WHERE NOT EXISTS (
                SELECT 1 FROM "MenuArticles" WHERE "MenuId" = duo_menu_id AND "ArticleId" = reine_id
            );
        END IF;

        IF bbq_burger_id IS NOT NULL THEN
            INSERT INTO "MenuArticles" ("MenuId", "ArticleId")
            SELECT duo_menu_id, bbq_burger_id
            WHERE NOT EXISTS (
                SELECT 1 FROM "MenuArticles" WHERE "MenuId" = duo_menu_id AND "ArticleId" = bbq_burger_id
            );
        END IF;

        IF fondant_id IS NOT NULL THEN
            INSERT INTO "MenuArticles" ("MenuId", "ArticleId")
            SELECT duo_menu_id, fondant_id
            WHERE NOT EXISTS (
                SELECT 1 FROM "MenuArticles" WHERE "MenuId" = duo_menu_id AND "ArticleId" = fondant_id
            );
        END IF;
    END IF;
END $$;
""");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
DELETE FROM "MenuArticles"
WHERE "MenuId" IN (SELECT "MenuId" FROM "Menus" WHERE "Title" IN ('Menu Gourmand', 'Menu Duo'));

DELETE FROM "Menus" WHERE "Title" IN ('Menu Gourmand', 'Menu Duo');
""");
        }
    }
}
