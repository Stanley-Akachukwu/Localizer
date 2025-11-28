namespace Lacalizer.Shared.Dtos;

//public class PaginatedVideos
//{
//    public int PageIndex { get; set; }
//    public int PageSize { get; set; }
//    public int Count { get; set; }
//    public List<VideoDto>? Data { get; set; }
//}
public class VideoDto
{
    public string? Id { get; set; }
    public string? Language { get; set; }
    public string? Title { get; set; }
    public string? Topic { get; set; }
    public string? VideoUri { get; set; }
}