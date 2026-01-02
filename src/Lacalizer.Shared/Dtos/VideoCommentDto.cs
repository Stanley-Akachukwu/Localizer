
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace Lacalizer.Shared.Dtos;

public class VideoCommentDto
{
    public string? VideoId { get; set; }
    public string? VideoTopicId { get; set; }
    public string Id { get; set; }
    public string? ParentId { get; set; } 
    public string? Author { get; set; }
    public string? Content { get; set; }
}
