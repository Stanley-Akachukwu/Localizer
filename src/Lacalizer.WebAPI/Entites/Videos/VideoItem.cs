using Lacalizer.Shared.Enums;
using Lacalizer.WebAPI.Entites.Users;

namespace Lacalizer.WebAPI.Entites.Videos;

public class VideoItem : BaseEntity<string>
{
    public VideoItem()
    {
        Id = NUlid.Ulid.NewUlid().ToString();
    }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public string Language { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string VideoUri { get; set; } = string.Empty;
    public string? VideoTopicId { get; set; } = NUlid.Ulid.Empty.ToString();
    public VideoTopic? VideoTopic { get; set; } 
    public VideoType VideoType { get; set; } = VideoType.PARTICIPATION;
    public int LikeCounts { get; set; }
    public int CommentCounts { get; set; }
    public int ShareCounts { get; set; }
    public int ParticipantCounts { get; set; }
    public ICollection<Comment>? Comments { get; set; }
}
