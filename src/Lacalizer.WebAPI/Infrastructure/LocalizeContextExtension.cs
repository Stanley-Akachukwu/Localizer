using Lacalizer.WebAPI.Entites.Users;
using Lacalizer.WebAPI.Entites.Videos;
using Lacalizer.WebAPI.Infrastructure.Seeds;
using Microsoft.AspNetCore.Identity;
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
                var dbContext = services.GetRequiredService<LocalizeDbContext>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

                //// ⚠️ DROP DATABASE (ONLY FOR DEV / ONE-TIME INIT)
                //dbContext.Database.EnsureDeleted();

                dbContext.Database.Migrate();

                DbInitializer.InitializeAsync(dbContext, userManager, cancellationToken).Wait();
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

    public static async Task InitializeAsync(LocalizeDbContext dbContext, UserManager<ApplicationUser> userManager, CancellationToken ct)
    {
        try
        {

            Console.WriteLine($"DB Initiating ...");
            var tables = new List<string>();
            Console.WriteLine($"Seeding ...");

            var userSeed = await UserSeedAsync(userManager, ct);

            if (userSeed)
            {
                tables.Add("userSeed");
            }


            var videoContextSeed = await VideoTopicSeedAsync(dbContext, ct);
            if (videoContextSeed)
            {
                if (videoContextSeed) tables.Add("videoContextSeed");
            }


            var videoItemSeed = await VideoItemSeedAsync(dbContext, ct);
            if (videoItemSeed)
            {
                if (videoItemSeed) tables.Add("videoItemSeed");
            }

            await dbContext.SaveChangesAsync();
            Console.WriteLine($"Seeded: {tables.Count} tables");
            
        }
        catch (NpgsqlException ex)
        {
            Console.Error.WriteLine($"Error Initializing db: {ex.Message}");
        }
    }

    private static async Task<bool> UserSeedAsync(
     UserManager<ApplicationUser> userManager,
     CancellationToken ct)
    {
        try
        {
            var users = UserSeed.GetUsers();

            foreach (var user in users)
            {
                var exists = await userManager.FindByIdAsync(user.Id);

                if (exists == null)
                {
                    var result = await userManager.CreateAsync(user);

                    if (!result.Succeeded)
                    {
                        Console.Error.WriteLine(
                            $"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                        return false;
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"User seeding error: {ex.Message}");
            return false;
        }
    }

    private static async Task<bool> VideoItemSeedAsync(LocalizeDbContext dbContext, CancellationToken ct)
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
                        VideoContextId = v.VideoContextId,
                        Description = "Localized Video",
                        Language = v.Language,
                        ContextText = v.ContextText,
                        VideoUri = v.VideoUri,
                        IsActive = true,
                        CreatedByUserId = v.CreatedByUserId,
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow,
                        UpdatedByUserId = v.UpdatedByUserId,
                        VideoType = v.VideoType,
                        UserId = v.UserId,
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
    private static async Task<bool> VideoTopicSeedAsync(LocalizeDbContext dbContext, CancellationToken ct)
    {
        
        try
        {
            Console.WriteLine($"Seeding videos roles...");
            var videoContexts = new List<VideoContext>();
            var topicSeeds = await VideoContextSeed.GetDefaultVideoContextsAsync(ct);

            foreach (var v in topicSeeds)
            {
                if (!await dbContext.VideoContexts.AnyAsync(i => i.Id == v.Id, ct))
                {
                    var videoTopic = new VideoContext
                    {
                        Id = v.Id!,
                        TargetLanguage = v.TargetLanguage,
                        ContextText = v.ContextText,
                        Description = "Text for translation",
                        IsActive = true,
                        CreatedByUserId = v.CreatedByUserId,
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow,
                        UpdatedByUserId = v.CreatedByUserId,
                        UserId = v.UserId,

                    };
                    videoContexts.Add(videoTopic);
                }
            }

            if (videoContexts.Any())
            {
                await dbContext.VideoContexts.AddRangeAsync(videoContexts, ct);
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Seeding Default videoTopics...: {ex.Message}");
        }

        return false;
    }
}
