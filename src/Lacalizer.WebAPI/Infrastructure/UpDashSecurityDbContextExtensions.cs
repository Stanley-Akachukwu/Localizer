using Lacalizer.WebAPI.Entites.Videos;
using Lacalizer.WebAPI.Infrastructure.Seeds;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Lacalizer.WebAPI.Infrastructure;
 
public static class LocalizeContextExtension
{
    public static void CreateLocalizeDbIfNotExists(this IHost host)
    {
        CancellationToken cancellationToken = default;
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var configuration = services.GetRequiredService<IConfiguration>();

                string sysInitId = configuration.GetValue<string>("LacalizeSettings:SysInitId")!;
                var dbContext = services.GetRequiredService<LocalizeContext>();

                dbContext.Database.Migrate();

                DbInitializer.InitializeAsync(dbContext, sysInitId, scope, cancellationToken).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB Init failed: {ex.Message}");
            }
        }
    }
}

 
public static class DbInitializer
{

    public static async Task InitializeAsync(LocalizeContext dbContext, string sysInitId, IServiceScope scope, CancellationToken ct)
    {
        try
        {

            Console.WriteLine($"DB Initiating ...");
            var tables = new List<string>();
            Console.WriteLine($"Seeding ...");


            var videoItemSeed = await VideoItemSeedAsync(dbContext, sysInitId, ct);
            dbContext.SaveChanges();
            if (videoItemSeed) tables.Add("videoItemSeed");
            Console.WriteLine($"Seeded: {tables.Count} tables");
        }
        catch (NpgsqlException ex)
        {
            Console.Error.WriteLine($"Error Initializing db: {ex.Message}");
        }
    }

    private static async Task<bool> VideoItemSeedAsync(LocalizeContext dbContext, string sysInitId, CancellationToken ct)
    {
        try
        {
            Console.WriteLine($"Seeding videos roles...");
            var videoItems = new List<VideoItem>();
            var videoSeeds = await VideoItemSeed.GetDefaultVideoItemAsync(ct);

            foreach (var v in videoSeeds)
            {
                if (!await dbContext.VideoItems.AnyAsync(i => i.Id == v.Id, ct))
                {
                    var videoItem = new VideoItem
                    {
                        Id = v.Id!,
                        Description = v.Title,
                        Language = v.Language,
                        Topic = v.Topic,
                        Title = v.Title,
                        VideoUri = v.VideoUri,
                        IsActive = true,
                        CreatedByUserId = v.CreatedByUserId,
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow,
                        UpdatedByUserId = v.UpdatedByUserId,
                        UID = v.VideoUri,
                        VideoType = v.VideoType
                    };

                    videoItems.Add(videoItem);
                }
            }

            if (videoItems.Any())
            {
                await dbContext.VideoItems.AddRangeAsync(videoItems, ct);
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Seeding Default videoItems...: {ex.Message}");
        }

        return false;
    }

}
