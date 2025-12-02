using Lacalizer.Shared.Enums;
using Lacalizer.WebAPI.Entites.Videos;
using NUlid;

namespace Lacalizer.WebAPI.Infrastructure.Seeds;
 
public static class VideoItemSeed
{
    private static readonly Ulid SystemUserId = Ulid.Empty;
    public static async Task<VideoItem[]> GetDefaultVideoItemAsync(CancellationToken ct)
    {
        return new[]
        {
                new VideoItem()
                {
                    Id = "1KAZM0HZ2JZPFBW643KT15T4G",
                    Language = "English",
                    VideoType = VideoType.TOPIC,
                    Title = "Eze Adi",
                    Topic = "Eze Adi hurried over his breakfast of cassava served with cold bitter-leaf soup. \r\nIt was all that remained of last night's supper.",
                    VideoUri = "https://github.com/ewerspej/maui-samples/blob/main/assets/bigbuckbunny.mp4?raw=true",
                    CreatedByUserId = SystemUserId.ToString(),
                    VideoTopicId = "TOPIC01KAZM0HZ2JZPFBW643KT15T4G"
                },
                new VideoItem()
                {
                    Id = "01KAZM0HZ2JZPFBW643KTHT234",
                    Language = "French",
                    VideoType = VideoType.TOPIC,
                    Title = "Eze Adi",
                    Topic = "Then he put away the bowls from which he and his mother had eaten, and set off to the village of Ama, three miles away. Eze was going to school for the first time.",
                    VideoUri = "https://github.com/ewerspej/maui-samples/blob/main/assets/bigbuckbunny.mp4?raw=true",
                    CreatedByUserId = SystemUserId.ToString(),
                    VideoTopicId = "TOPIC01KAZM0HZ2JZPFBW643KT15T4G",

                },
                new VideoItem()
                {
                    Id = "TOPIC01KAZM0HZ2JZPFBW643KT4THKE",
                    Language = "Spanish",
                    VideoType = VideoType.TOPIC,
                    Title = "Machine",
                    Topic = "A simulator is a machine, program, or device that imitates a real-life situation, typically for training, experimentation, or entertainment.",
                    VideoUri = "https://github.com/ewerspej/maui-samples/blob/main/assets/bigbuckbunny.mp4?raw=true",
                    CreatedByUserId = SystemUserId.ToString(),
                    VideoTopicId = "TOPIC01KAZM0HZ2JZPFBW643KT15T4G"

                },
               
        };
    }
}

