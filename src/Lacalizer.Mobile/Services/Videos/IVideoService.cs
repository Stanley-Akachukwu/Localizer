using Lacalizer.Mobile.Models;
using Lacalizer.Mobile.Services.Users;
using Lacalizer.Shared.Dtos;
using Lacalizer.Shared.Enums;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Lacalizer.Mobile.Services.Videos;

public interface IVideoService
{
    Task<List<ReelVideoModel>> GetTopicVideosAsync(int pageIndex, int pageSize, CancellationToken ct = default);
    Task<ReelVideoModel?> CreateVideoAsync(VideoCreateRequest request, CancellationToken ct = default);
    Task<List<ParticipationVideoModel>> GetParticipationVideosAsync(int pageIndex, int pageSize, string? videoTopicId, CancellationToken ct = default);
    Task<int?> SaveLikeAsync(string videoItemId, CancellationToken ct = default);
    Task<int?> SaveParticipationCountAsync(string videoItemId, CancellationToken ct = default);
}

public class VideoService : IVideoService
{
    private readonly IApiClient _apiClient;
    private readonly IMemoryCache _cache;
    private IConfiguration _config;
    public VideoService(IApiClient apiClient, IMemoryCache cache, IConfiguration config)
    {
        _apiClient = apiClient;
        _cache = cache;
        _config = config; 
    }

    public async Task<List<ReelVideoModel>> GetTopicVideosAsync(
    int pageIndex,
    int pageSize,
    CancellationToken ct = default)
    {
        try
        {
            string cacheKey = $"videos-{pageIndex}-{pageSize}-{VideoType.CONTEXT}";

            if (_cache.TryGetValue(cacheKey, out List<ReelVideoModel> cachedVideos))
                return cachedVideos!;

            var url = $"api/videoitems?PageIndex={pageIndex}&PageSize={pageSize}&VideoType={VideoType.CONTEXT}";

            
            var response = await _apiClient
      .GetAsync<LocalizerApiResponse<PaginatedItems<VideoDto>>>(url, ct);


            if (response?.Data?.Data is null || response?.Data?.Data is null)
                return new List<ReelVideoModel>();


            var items = response?.Data.Data
                .Select(v => new ReelVideoModel(
                    v.ContextText!,
                    v.VideoUri!,
                    v.VideoContextId!,v.SavedLikes,v.SavedComments,v.SavedShares, v.SavedParticipants,v.Id!, null!, null!, null!,null!))
                .ToList();

            _cache.Set(cacheKey, items, TimeSpan.FromMinutes(10));

            return items;
        }
        catch (TaskCanceledException)
        {
            return new List<ReelVideoModel>();
        }
        catch (Exception exp)
        {
            return new List<ReelVideoModel>();
        }
    }

    public async Task<List<ParticipationVideoModel>> GetParticipationVideosAsync(
    int pageIndex,
    int pageSize,string? videoContextId,
    CancellationToken ct = default)
    {
        try
        {
            var url = $"api/videoitems?PageIndex={pageIndex}&PageSize={pageSize}&VideoType={VideoType.PARTICIPATION}&VideoContextId={videoContextId}";
            var response = await _apiClient
               .GetAsync<LocalizerApiResponse<PaginatedItems<VideoDto>>>(url, ct);


            if (response?.Data?.Data is null || response?.Data?.Data is null)
                return new List<ParticipationVideoModel>();


            var items = response?.Data.Data
                .Select(v => new ParticipationVideoModel(
                    v.ContextText!,
                    v.VideoUri!,
                    v.VideoContextId!, v.SavedLikes, v.SavedComments, v.SavedShares, v.SavedParticipants, v.Id!, null!, null!, null!, null!))
                .ToList();
            return items;
        }
        catch (TaskCanceledException)
        {
            return new List<ParticipationVideoModel>();
        }
        catch (Exception exp)
        {
            return new List<ParticipationVideoModel>();
        }
    }
    public async Task<ReelVideoModel?> CreateVideoAsync(
        VideoCreateRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var response = await _apiClient.PostAsync<VideoCreateRequest, LocalizerApiResponse<VideoDto>>("api/videoitems", request);

            if (response == null || response.Data == null)
                return null;

            return new ReelVideoModel(
                response.Data.ContextText!,
                response.Data.VideoUri!,
                response.Data.Id!, response.Data.SavedLikes, response.Data.SavedComments, response.Data.SavedShares, response.Data.SavedParticipants, response.Data.Id!, null!, null!, null!, null!);
        }
        catch (TaskCanceledException)
        {
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<int?> SaveLikeAsync(string videoItemId, CancellationToken ct = default)
    {
        var request = new LikeVideoRequest(videoItemId);
        var url = "api/videoitems/saveLike";

        try
        {
            var response = await _apiClient.PostAsync<LikeVideoRequest, LocalizerApiResponse<int>>("api/videoitems", request);

            if (response == null || response.Data == null)
                return null;

            return response?.Data;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public async Task<int?> SaveParticipationCountAsync(string videoItemId, CancellationToken ct = default)
    {
        var request = new LocalizeParticipationRequest(videoItemId);

        try
        {
            var response = await _apiClient.PostAsync<LocalizeParticipationRequest, LocalizerApiResponse<int>>("api/videoitems/saveParticipation", request);

            if (response == null || response.Data == null)
                return null;

            return response?.Data;
        }
        catch (Exception)
        {
            return 0;
        }
    }
}

public record VideoCreateRequest(string ContextText, string VideoUri, string Language ,string VideoContextId);
public record LikeVideoRequest(string videoItemId);
public record LocalizeParticipationRequest(string videoItemId);


