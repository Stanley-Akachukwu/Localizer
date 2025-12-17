
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace Lacalizer.Shared.Dtos;

public class VideoCommentDto
{
    public string? VideoId { get; set; }
    public string? VideoTopicId { get; set; }

    public int Id { get; set; }
    public int? ParentId { get; set; } 
    public string? Author { get; set; }
    public string? Content { get; set; }
    public ObservableCollection<VideoCommentDto> Children { get; set; } = new ObservableCollection<VideoCommentDto>();
    public int Depth { get; set; }
}
