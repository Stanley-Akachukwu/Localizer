using Lacalizer.WebAPI.Entites.Videos;
namespace Lacalizer.WebAPI.Infrastructure.Seeds;

public static class VideoContextSeed
{
    public static readonly string SystemUserId = UserSeed.SystemUser.Id;
    public static async Task<VideoContext[]> GetDefaultVideoContextsAsync(CancellationToken ct)
    {       
        return new[]
        {
                new VideoContext()
                {
                    Id = "TOPIC01KAZM0HZ2JZPFBW643KT15T4G",
                     UserId = "01SYSTEMUSER00000000000001",
                    TargetLanguage = "Igbo",
                    ContextText = "Eze Adi hurried over his breakfast of cassava served with cold bitter-leaf soup. \r\nIt was all that remained of last night's supper.",
                    CreatedByUserId = SystemUserId.ToString(),
                },
                new VideoContext()
                {
                    Id = "TOPIC01KAZM0HZ2MVWNHJZ7FVV0V27E",
                     UserId = "01SYSTEMUSER00000000000001",
                    TargetLanguage = "Igbo",
                    ContextText = "Then he put away the bowls from which he and his mother had eaten, and set off to the village of Ama, three miles away. Eze was going to school for the first time.",
                    CreatedByUserId = SystemUserId.ToString(),
                } 
        };
    }

}

