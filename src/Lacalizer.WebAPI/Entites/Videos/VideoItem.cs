using Lacalizer.Shared.Enums;

namespace Lacalizer.WebAPI.Entites.Videos;

public class VideoItem : BaseEntity<string>
{
    public VideoItem()
    {
        Id = NUlid.Ulid.NewUlid().ToString();
    }

    public string Language { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string VideoUri { get; set; } = string.Empty;
    public VideoType VideoType { get; set; } = VideoType.PARTICIPATION;
}
