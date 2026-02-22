
using Lacalizer.Mobile.Models;
using Lacalizer.Shared.Dtos;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;

namespace Lacalizer.Mobile.Services.Comments;

public interface ICommentService
{
    Task<List<VideoComment>> GetVideoCommentsAsync(
    int pageIndex,
    int pageSize, string videoItemId,
    CancellationToken ct = default);
    Task<VideoComment> PostCommentAsync(VideoComment commentDto, CancellationToken ct = default);
}

public class CommentService : ICommentService
{
    private readonly HttpClient _client;
    private readonly IMemoryCache _cache;
    private IConfiguration _config;
    public CommentService(HttpClient client, IMemoryCache cache, IConfiguration config)
    {
        _client = client;
        _cache = cache;
        _config = config;
        _client.BaseAddress = new Uri(_config["ApiSettings:BaseUrl"]);
    }

    public async Task<List<VideoComment>> GetVideoCommentsAsync( 
    int pageIndex,
    int pageSize, string videoItemId,
    CancellationToken ct = default) 
    {
        try
        {
            var url = $"api/comments?PageIndex={pageIndex}&PageSize={pageSize}&videoItemId={videoItemId}";

            using var response = await _client.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(ct);
            if (string.IsNullOrWhiteSpace(content))
                return new List<VideoComment>();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var rsp = await response.Content
                .ReadFromJsonAsync<LocalizerApiResponse<PaginatedItems<VideoCommentDto>>>(options, ct);

            if (rsp is null || rsp.Data is null)
                return new List<VideoComment>();
           
            var items = rsp.Data.Data
                .Select(c => new VideoComment
                {
                    Id = c?.Id?.ToString(),
                    Author = c?.Author,
                    Content = c?.Content,
                    ParentId = c?.ParentId?.ToString(),
                })
                .ToList();

            return items;
        }
        catch (TaskCanceledException)
        {
            return new List<VideoComment>();
        }
        catch (Exception exp)
        {
            return new List<VideoComment>();
        }
    }

    public async Task<VideoComment> PostCommentAsync(VideoComment videoComment, CancellationToken ct = default)
    {
        var request = new SaveVideoRequest(videoComment.VideoId,videoComment.ParentId,videoComment.Author,videoComment.Content);

        var url = "api/comments/saveComment";

        try
        {
            using var response = await _client.PostAsJsonAsync(url, request, ct);

            response.EnsureSuccessStatusCode();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var rsp = await response.Content
                .ReadFromJsonAsync<LocalizerApiResponse<CreateCommentResult>>(options, ct);
            if (rsp.Data is not null && rsp.IsSuccess)
            {
                videoComment.Id = rsp.Data.CommentId;
                videoComment.CommentCount = rsp.Data.CommentCount;
            }

                return await Task.FromResult(videoComment);
        }
        catch (Exception)
        {
            return await Task.FromResult(videoComment);
        }
        
    }
    
}

public record SaveVideoRequest(string videoItemId, string parentId, string author, string content);

