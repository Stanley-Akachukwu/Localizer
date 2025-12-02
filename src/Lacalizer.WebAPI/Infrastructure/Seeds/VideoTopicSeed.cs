using Lacalizer.WebAPI.Entites.Videos;
using NUlid;

namespace Lacalizer.WebAPI.Infrastructure.Seeds;

public static class VideoTopicSeed
{
    private static readonly Ulid SystemUserId = Ulid.Empty;
    public static async Task<VideoTopic[]> GetDefaultVideoTopicAsync(CancellationToken ct)
    {       
        return new[]
        {
                new VideoTopic()
                {
                    Id = "TOPIC01KAZM0HZ2JZPFBW643KT15T4G",
                    TargetLanguage = "Igbo",
                    Title = "Eze Adi",
                    Topic = "Eze Adi hurried over his breakfast of cassava served with cold bitter-leaf soup. \r\nIt was all that remained of last night's supper.",
                    CreatedByUserId = SystemUserId.ToString(),
                },
                new VideoTopic()
                {
                    Id = "TOPIC01KAZM0HZ2MVWNHJZ7FVV0V27E",
                    TargetLanguage = "Igbo",
                    Title = "Eze Adi",
                    Topic = "Then he put away the bowls from which he and his mother had eaten, and set off to the village of Ama, three miles away. Eze was going to school for the first time.",
                    CreatedByUserId = SystemUserId.ToString(),
                } 
        };
    }

}

