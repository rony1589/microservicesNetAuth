using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ProductsService.Infrastructure;

public class ProductsDbContextFactory : IDesignTimeDbContextFactory<ProductsDbContext>
{
    public ProductsDbContext CreateDbContext(string[] args)
    {
        var webDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "ProductsService.Web"));

        var cfg = new ConfigurationBuilder()
            .SetBasePath(webDir)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var cs = cfg.GetConnectionString("ProductsDb");
        if (string.IsNullOrWhiteSpace(cs))
            throw new InvalidOperationException("Falta ConnectionStrings:ProductsDb en el appsettings.json del proyecto Web (ProductsService.Web).");

        var opts = new DbContextOptionsBuilder<ProductsDbContext>()
            .UseNpgsql(cs)
            .Options;

        return new ProductsDbContext(opts);
    }
}
