using Lacalizer.Mobile.Models;
using Lacalizer.Shared.Dtos;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;

namespace Lacalizer.Mobile.Services.Videos;


public interface IContextService
{
    Task<LocalizerApiResponse<PaginatedItems<ContextModel>>> GetContextsAsync(
    int pageIndex,
    int pageSize, string contextId,
    CancellationToken ct = default);
    Task<ContextModel> PostContextAsync(ContextModel contextModel, string userId, CancellationToken ct = default);
}

public class ContextService : IContextService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private IConfiguration _config;
    public ContextService(HttpClient httpClient, IMemoryCache cache, IConfiguration config)
    {
        _httpClient = httpClient;
        _cache = cache;
        _config = config;
       // _client.BaseAddress = new Uri(_config["ApiSettings:BaseUrl"]);
    }

    
    public async Task<LocalizerApiResponse<PaginatedItems<ContextModel>>> GetContextsAsync(int pageIndex,
    int pageSize, string contextItemId,
    CancellationToken ct = default)
    {
        try
        {
            var url = $"api/contexts?PageIndex={pageIndex}&PageSize={pageSize}&contextId={contextItemId}";

            using var response = await _httpClient.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(ct);
            if (string.IsNullOrWhiteSpace(content))
                return new LocalizerApiResponse<PaginatedItems<ContextModel>>();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var rsp = await response.Content
                .ReadFromJsonAsync<LocalizerApiResponse<PaginatedItems<ContextModel>>>(options, ct);

            if (rsp is null || rsp.Data is null)
                return new LocalizerApiResponse<PaginatedItems<ContextModel>>();

            var items = rsp.Data.Data
                .Select(c => new ContextModel
                {
                    Id = c?.Id?.ToString(),
                    ContextText = c?.ContextText!,
                    CreatedAt = c.CreatedAt,
                })
                .ToList();

            var pagedItems = items
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new PaginatedItems<ContextModel>(
                pageIndex,
                pageSize,
                items.Count,
                pagedItems);

            return LocalizerApiResponse<PaginatedItems<ContextModel>>
          .Success(result, 200);
        }
        catch (TaskCanceledException)
        {
            return new LocalizerApiResponse<PaginatedItems<ContextModel>>();
        }
        catch (Exception exp)
        {
            return new LocalizerApiResponse<PaginatedItems<ContextModel>>();
        }
    }

    public async Task<ContextModel> PostContextAsync(ContextModel context, string userId, CancellationToken ct = default)
    {
        var request = new SaveContextRequest(context.ContextText, userId);

        var url = "api/contexts/saveContext";

        try
        {
            using var response = await _httpClient.PostAsJsonAsync(url, request, ct);

            response.EnsureSuccessStatusCode();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var rsp = await response.Content
                .ReadFromJsonAsync<LocalizerApiResponse<CreateContextResult>>(options, ct);
            if (rsp.Data is not null && rsp.IsSuccess)
            {
                context.Id = rsp.Data.Id;
                context.ContextText = rsp.Data.ContextText;
            }

            return await Task.FromResult(context);
        }
        catch (Exception ex)
        {
            return await Task.FromResult(context);
        }

    }

}
public record SaveContextRequest(string ContextText, string userId);