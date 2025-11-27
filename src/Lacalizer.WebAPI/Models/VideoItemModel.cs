namespace Lacalizer.WebAPI.Models;

public class VideoItemModel
{
    public string Id { get; set; } = NUlid.Ulid.NewUlid().ToString();
    public string Language { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string BlobName { get; set; } = string.Empty;
}

