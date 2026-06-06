
using Lacalizer.Mobile.Models;
using Lacalizer.Mobile.Services.Users;
using Lacalizer.Shared.Dtos;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

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
    private readonly IMemoryCache _cache;
    private IConfiguration _config;
    private readonly IApiClient _apiClient;

    public CommentService(IApiClient apiClient, IMemoryCache cache, IConfiguration config)
    {
        _apiClient = apiClient;
        _cache = cache;
        _config = config;
    }

    public async Task<List<VideoComment>> GetVideoCommentsAsync( 
    int pageIndex,
    int pageSize, string videoItemId,
    CancellationToken ct = default) 
    {
        try
        {
            var url = $"api/comments?PageIndex={pageIndex}&PageSize={pageSize}&videoItemId={videoItemId}";

            var response = await _apiClient
        .GetAsync<LocalizerApiResponse<PaginatedItems<VideoCommentDto>>>(url, ct);


            if (response?.Data?.Data is null || response?.Data?.Data is null)
                return new List<VideoComment>();
           
            var items = response?.Data.Data
                .Select(c => new VideoComment
                {
                    Id = c?.Id?.ToString(),
                    Author = c?.Author,
                    ContentText = c?.Content,
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
        var request = new SaveVideoRequest(videoComment.VideoId,videoComment.ParentId,videoComment.Author,videoComment.ContentText); 

        try
        {
            var result = await _apiClient.PostAsync<SaveVideoRequest, VideoComment>("api/comments/saveComment", request);
            return await Task.FromResult(videoComment);
        }
        catch (Exception)
        {
            return await Task.FromResult(videoComment);
        }
        
    }
    
}

public record SaveVideoRequest(string videoItemId, string parentId, string author, string content);

