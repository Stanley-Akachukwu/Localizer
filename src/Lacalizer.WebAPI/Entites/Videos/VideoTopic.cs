using Lacalizer.WebAPI.Entites.Users;

namespace Lacalizer.WebAPI.Entites.Videos;

public class VideoTopic : BaseEntity<string>
{
    public VideoTopic()
    {
        Id = NUlid.Ulid.NewUlid().ToString();
    }

    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public string TargetLanguage { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public ICollection<VideoItem>? VideoItems { get; set; }
}
