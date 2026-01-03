using Lacalizer.Mobile.Models;
using Lacalizer.Shared.Dtos;
using Lacalizer.Shared.Enums;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;
using System.Text.Json;

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
    private readonly HttpClient _client;
    private readonly IMemoryCache _cache;

    public VideoService(HttpClient client, IMemoryCache cache)
    {
        _client = client;
        _cache = cache;
    }

    public async Task<List<ReelVideoModel>> GetTopicVideosAsync(
    int pageIndex,
    int pageSize,
    CancellationToken ct = default)
    {
        try
        {
            string cacheKey = $"videos-{pageIndex}-{pageSize}-{VideoType.TOPIC}";

            if (_cache.TryGetValue(cacheKey, out List<ReelVideoModel> cachedVideos))
                return cachedVideos;

            var url = $"api/videoitems?PageIndex={pageIndex}&PageSize={pageSize}&VideoType={VideoType.TOPIC}";

            using var response = await _client.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(ct);
            if (string.IsNullOrWhiteSpace(content))
                return new List<ReelVideoModel>();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var rsp = await response.Content
                .ReadFromJsonAsync<LocalizerApiResponse<PaginatedItems<VideoDto>>>(options, ct);

            if (rsp is null || rsp.Data is null)
                return new List<ReelVideoModel>();

            var items = rsp.Data.Data
                .Select(v => new ReelVideoModel(
                    v.Title,
                    v.Topic,
                    v.VideoUri,
                    v.VideoTopicId,v.SavedLikes,v.SavedComments,v.SavedShares, v.SavedParticipants,v.Id, null, null, null,null))
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
    int pageSize,string? videoTopicId,
    CancellationToken ct = default)
    {
        try
        {
            var url = $"api/videoitems?PageIndex={pageIndex}&PageSize={pageSize}&VideoType={VideoType.PARTICIPATION}&VideoTopicId={videoTopicId}";

            using var response = await _client.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(ct);
            if (string.IsNullOrWhiteSpace(content))
                return new List<ParticipationVideoModel>();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var rsp = await response.Content
                .ReadFromJsonAsync<LocalizerApiResponse<PaginatedItems<VideoDto>>>(options, ct);

            if (rsp is null || rsp.Data is null)
                return new List<ParticipationVideoModel>();

            var items = rsp.Data.Data
                .Select(v => new ParticipationVideoModel(
                    v.Title,
                    v.Topic,
                    v.VideoUri,
                    v.Id
                ))
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
            var url = "api/videoitems";

            using var response = await _client.PostAsJsonAsync(url, request, ct);

            response.EnsureSuccessStatusCode();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var rsp = await response.Content
                .ReadFromJsonAsync<LocalizerApiResponse<VideoDto>>(options, ct);

            if (rsp == null || rsp.Data == null)
                return null;

            return new ReelVideoModel(
                rsp.Data.Title,
                rsp.Data.Topic,
                rsp.Data.VideoUri,
                rsp.Data.Id, rsp.Data.SavedLikes, rsp.Data.SavedComments, rsp.Data.SavedShares, rsp.Data.SavedParticipants, rsp.Data.Id, null, null, null, null);
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
            using var response = await _client.PostAsJsonAsync(url, request, ct);

            response.EnsureSuccessStatusCode();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var rsp = await response.Content
                .ReadFromJsonAsync<LocalizerApiResponse<int>>(options, ct);

            return rsp.Data;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public async Task<int?> SaveParticipationCountAsync(string videoItemId, CancellationToken ct = default)
    {
        var request = new LocalizeParticipationRequest(videoItemId);
        var url = "api/videoitems/saveParticipation";

        try
        {
            using var response = await _client.PostAsJsonAsync(url, request, ct);

            response.EnsureSuccessStatusCode();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var rsp = await response.Content
                .ReadFromJsonAsync<LocalizerApiResponse<int>>(options, ct);

            return rsp.Data;
        }
        catch (Exception)
        {
            return 0;
        }
    }
}

public record VideoCreateRequest(string Title, string Topic, string VideoUri, string Language ,string TopicId);
public record LikeVideoRequest(string videoItemId);
public record LocalizeParticipationRequest(string videoItemId);


