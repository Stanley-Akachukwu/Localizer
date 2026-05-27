using Lacalizer.WebAPI.Entites.Users;
using Lacalizer.WebAPI.Entites.Videos;

namespace Lacalizer.WebAPI.Entites;

public class Comment : BaseEntity<string>
{
    public Comment()
    {
        Id = NUlid.Ulid.NewUlid().ToString();
    }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public string? VideoItemId { get; set; }
    public VideoItem? VideoItem { get; set; }
    public string? VideoTopicId { get; set; }
    public string? ParentId { get; set; }
    public string? Author { get; set; }
    public string? Content { get; set; }
}
