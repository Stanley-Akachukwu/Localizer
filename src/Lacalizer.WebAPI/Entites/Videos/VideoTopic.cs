namespace Lacalizer.WebAPI.Entites.Videos;

public class VideoTopic : BaseEntity<string>
{
    public VideoTopic()
    {
        Id = NUlid.Ulid.NewUlid().ToString();
    }

    public string TargetLanguage { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public ICollection<VideoItem>? VideoItems { get; set; }
}
