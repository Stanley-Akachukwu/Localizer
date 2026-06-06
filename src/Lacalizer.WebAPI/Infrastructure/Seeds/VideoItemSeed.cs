using Lacalizer.Shared.Enums;
using Lacalizer.WebAPI.Entites.Videos;
using NUlid;

namespace Lacalizer.WebAPI.Infrastructure.Seeds;
 
public static class VideoItemSeed
{
    public static readonly string SystemUserId = UserSeed.SystemUser.Id;
    public static async Task<VideoItem[]> GetDefaultVideoItemAsync(CancellationToken ct)
    {
        return new[]
        {
                new VideoItem()
                {
                    Id = "1KAZM0HZ2JZPFBW643KT15T4G",
                    UserId = "01SYSTEMUSER00000000000001",
                    Language = "English",
                    VideoType = VideoType.CONTEXT,
                    ContextText = "Eze Adi hurried over his breakfast of cassava served with cold bitter-leaf soup. \r\nIt was all that remained of last night's supper.",
                    VideoUri = "https://github.com/ewerspej/maui-samples/blob/main/assets/bigbuckbunny.mp4?raw=true",
                    CreatedByUserId = SystemUserId.ToString(),
                    VideoContextId = "TOPIC01KAZM0HZ2JZPFBW643KT15T4G"
                },
                new VideoItem()
                {
                    Id = "01KAZM0HZ2JZPFBW643KTHT234",
                     UserId = "01SYSTEMUSER00000000000001",
                    Language = "French",
                    VideoType = VideoType.CONTEXT,
                    ContextText = "Then he put away the bowls from which he and his mother had eaten, and set off to the village of Ama, three miles away. Eze was going to school for the first time.",
                    VideoUri = "https://github.com/ewerspej/maui-samples/blob/main/assets/bigbuckbunny.mp4?raw=true",
                    CreatedByUserId = SystemUserId.ToString(),
                    VideoContextId = "TOPIC01KAZM0HZ2JZPFBW643KT15T4G",

                },
                new VideoItem()
                {
                    Id = "TOPIC01KAZM0HZ2JZPFBW643KT4THKE",
                     UserId = "01SYSTEMUSER00000000000001",
                    Language = "Spanish",
                    VideoType = VideoType.CONTEXT,
                    ContextText = "A simulator is a machine, program, or device that imitates a real-life situation, typically for training, experimentation, or entertainment.",
                    VideoUri = "https://github.com/ewerspej/maui-samples/blob/main/assets/bigbuckbunny.mp4?raw=true",
                    CreatedByUserId = SystemUserId.ToString(),
                    VideoContextId = "TOPIC01KAZM0HZ2JZPFBW643KT15T4G"

                },
               
        };
    }
}
