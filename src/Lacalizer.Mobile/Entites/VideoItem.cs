namespace Lacalizer.Mobile.Entites;

public class VideoItem : BaseEntity<string>
{
    protected VideoItem()
    {
        Id = NUlid.Ulid.NewUlid().ToString();
    }

    public string Language { get; } = string.Empty;
    public string Title { get; } = string.Empty;
    public string Topic { get; } = string.Empty;
    public string VideoUri { get; } = string.Empty;
}
