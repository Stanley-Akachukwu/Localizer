using Lacalizer.WebAPI.Entites.Users;

namespace Lacalizer.WebAPI.Entites.Videos;

public class VideoContext : BaseEntity<string>
{
    public VideoContext()
    {
        Id = NUlid.Ulid.NewUlid().ToString();
    }

    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public string TargetLanguage { get; set; } = string.Empty;
    public string ContextText { get; set; } = string.Empty;
    public ICollection<VideoItem>? VideoItems { get; set; }
}
