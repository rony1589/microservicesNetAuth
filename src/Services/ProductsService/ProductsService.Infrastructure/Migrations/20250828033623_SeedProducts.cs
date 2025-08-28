using Microsoft.EntityFrameworkCore.Migrations;
using System.Globalization;

#nullable disable

namespace ProductsService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string Esc(string s) => s.Replace("'", "''");

            var items = new[]
            {
                new { Name = "Teclado Mecánico",  Description = "Switches azules, retroiluminado", Price = 199.99m, Category = "Periféricos", Status = true },
                new { Name = "Mouse Inalámbrico", Description = "2.4G + BT, 1600 DPI",            Price = 59.90m,  Category = "Periféricos", Status = true },
                new { Name = "Monitor 27''",      Description = "QHD, 144Hz",                     Price = 1299.00m,Category = "Monitores",   Status = true }
            };

            foreach (var p in items)
            {
                var price = p.Price.ToString(CultureInfo.InvariantCulture);
                migrationBuilder.Sql($@"
                INSERT INTO products (name, description, price, category, status)
                SELECT '{Esc(p.Name)}', '{Esc(p.Description)}', {price}, '{Esc(p.Category)}', {p.Status}
                WHERE NOT EXISTS (SELECT 1 FROM products WHERE name = '{Esc(p.Name)}');");
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE FROM products WHERE name IN ('Teclado Mecánico','Mouse Inalámbrico','Monitor 27''''');");
        }
    }
}
