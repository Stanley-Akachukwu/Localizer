
using Lacalizer.Shared.Dtos;
using Microsoft.Extensions.Caching.Memory;

namespace Lacalizer.Mobile.Services.Comments;

public interface ICommentService
{
    Task PostCommentAsync(VideoCommentDto commentDto);
}

public class CommentService : ICommentService
{
    private readonly HttpClient _client;
    private readonly IMemoryCache _cache;

    public CommentService(HttpClient client, IMemoryCache cache)
    {
        _client = client;
        _cache = cache;
    }
    public async Task PostCommentAsync(VideoCommentDto commentDto)
    {

        //var response = await _client.PostAsJsonAsync("/api/comments", payload);

        //response.EnsureSuccessStatusCode();
    }
}