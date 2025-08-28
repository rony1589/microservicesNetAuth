using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace UsersService.Infrastructure;

public class UsersDbContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
{
    public UsersDbContext CreateDbContext(string[] args)
    {
        // BasePath apuntando al proyecto Web (donde está el appsettings.json que ya tienes)
        var webDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "UsersService.Web"));

        var cfg = new ConfigurationBuilder()
            .SetBasePath(webDir)
            .AddJsonFile("appsettings.json", optional: false)
            // Si usas perfiles, puedes habilitar el siguiente:
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var cs = cfg.GetConnectionString("UsersDb");
        if (string.IsNullOrWhiteSpace(cs))
            throw new InvalidOperationException("Falta ConnectionStrings:UsersDb en el appsettings.json del proyecto Web (UsersService.Web).");

        var opts = new DbContextOptionsBuilder<UsersDbContext>()
            .UseNpgsql(cs)
            .Options;

        return new UsersDbContext(opts);
    }
}
