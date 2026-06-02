using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Lacalizer.WebAPI.Infrastructure;

public class LocalizeDbContextFactory : IDesignTimeDbContextFactory<LocalizeDbContext>
{
    public LocalizeDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LocalizeDbContext>();

        optionsBuilder.UseNpgsql(
            "localizedb",
            optsBuilder =>
            {
                //optsBuilder.MigrationsAssembly(ThisAssembly.Info.Title);

                optsBuilder.MigrationsHistoryTable("__EFMigrationsHistory", "public");
            });

        return new LocalizeDbContext(optionsBuilder.Options);
    }
}

 