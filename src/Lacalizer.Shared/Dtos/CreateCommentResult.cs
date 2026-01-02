

namespace Lacalizer.Shared.Dtos;

public class CreateCommentResult
{
    public string CommentId { get; set; } = string.Empty;
    public string VideoItemId { get; set; } = string.Empty;
    public int CommentCount { get; set; }
}