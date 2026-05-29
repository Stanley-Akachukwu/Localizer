using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Lacalizer.WebAPI.Infrastructure;

public class LocalizeDbContextFactory : IDesignTimeDbContextFactory<LocalizeContext>
{
    public LocalizeContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LocalizeContext>();

        optionsBuilder.UseNpgsql(
            "localizedb",
            optsBuilder =>
            {
                //optsBuilder.MigrationsAssembly(ThisAssembly.Info.Title);

                optsBuilder.MigrationsHistoryTable("__EFMigrationsHistory", "public");
            });

        return new LocalizeContext(optionsBuilder.Options);
    }
}

 