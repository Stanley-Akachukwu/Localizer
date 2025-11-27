using System;
using System.Collections.Generic;
using System.Text;

namespace Lacalizer.Mobile.Models;

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public T Data { get; set; }
    public string ResponseMessage { get; set; }
    public string ErrorMessage { get; set; }
    public object Errors { get; set; }
    public int StatusCode { get; set; }
}
public class PaginatedVideos
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int Count { get; set; }
    public List<VideoDto>? Data { get; set; }
}
public class VideoDto
{
    public string? Id { get; set; }
    public string? Language { get; set; }
    public string? Title { get; set; }
    public string? Topic { get; set; }
    public string? VideoUri { get; set; }   
}
