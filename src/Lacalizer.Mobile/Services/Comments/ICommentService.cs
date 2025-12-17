
using Lacalizer.Mobile.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Lacalizer.Mobile.Services.Comments;

public interface ICommentService
{
    Task<VideoComment> PostCommentAsync(VideoComment commentDto);
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
    public async Task<VideoComment> PostCommentAsync(VideoComment commentDto)
    {

        //var response = await _client.PostAsJsonAsync("/api/comments", payload);

        //response.EnsureSuccessStatusCode();
        return await Task.FromResult(commentDto);
    }
}