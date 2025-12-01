using Lacalizer.Mobile.Models;
using Lacalizer.Shared.Dtos;
using Lacalizer.Shared.Enums;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;
using System.Text.Json;

namespace Lacalizer.Mobile.Services.Videos;

public interface IVideoService
{
    Task<List<VideoModel>> GetTopicVideosAsync(int pageIndex, int pageSize, CancellationToken ct = default);
    Task<VideoModel?> CreateVideoAsync(VideoCreateRequest request, CancellationToken ct = default);
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

    public async Task<List<VideoModel>> GetTopicVideosAsync(
    int pageIndex,
    int pageSize,
    CancellationToken ct = default)
    {
        try
        {
            string cacheKey = $"videos-{pageIndex}-{pageSize}";

            if (_cache.TryGetValue(cacheKey, out List<VideoModel> cachedVideos))
                return cachedVideos;

            var url = $"api/videoitems?PageIndex={pageIndex}&PageSize={pageSize}&VideoType={VideoType.TOPIC}";

            using var response = await _client.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(ct);
            if (string.IsNullOrWhiteSpace(content))
                return new List<VideoModel>();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var rsp = await response.Content
                .ReadFromJsonAsync<LocalizerApiResponse<PaginatedItems<VideoDto>>>(options, ct);

            if (rsp is null || rsp.Data is null)
                return new List<VideoModel>();

            var items = rsp.Data.Data
                .Select(v => new VideoModel(
                    v.Title,
                    v.Topic,
                    v.VideoUri
                ))
                .ToList();

            _cache.Set(cacheKey, items, TimeSpan.FromMinutes(10));

            return items;
        }
        catch (TaskCanceledException)
        {
            return new List<VideoModel>();
        }
        catch (Exception exp)
        {
            return new List<VideoModel>();
        }
    }

    public async Task<VideoModel?> CreateVideoAsync(
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

            return new VideoModel(
                rsp.Data.Title,
                rsp.Data.Topic,
                rsp.Data.VideoUri
            );
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

}

public record VideoCreateRequest(string Title, string Topic, string VideoUri, string Language ="Igbo");

 