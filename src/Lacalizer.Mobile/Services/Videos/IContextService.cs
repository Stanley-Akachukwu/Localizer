using Lacalizer.Mobile.Models;
using Lacalizer.Mobile.Services.Users;
using Lacalizer.Shared.Dtos;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Lacalizer.Mobile.Services.Videos;


public interface IContextService
{
    Task<LocalizerApiResponse<PaginatedItems<ContextModel>>> GetContextsAsync(
    int pageIndex,
    int pageSize, string contextId,
    CancellationToken ct = default);
    Task<LocalizerApiResponse<ContextModel>> PostContextAsync(ContextModel contextModel, string userId, string targetLanguage, CancellationToken ct = default);
}

public class ContextService : IContextService
{
    private readonly IApiClient _apiClient;
    private readonly IMemoryCache _cache;
    private IConfiguration _config;
    public ContextService(IApiClient apiClient, IMemoryCache cache, IConfiguration config)
    {
        _apiClient = apiClient;
        _cache = cache;
        _config = config;
    }

    
    public async Task<LocalizerApiResponse<PaginatedItems<ContextModel>>> GetContextsAsync(int pageIndex,
    int pageSize, string contextItemId,
    CancellationToken ct = default)
    {
        try
        {
            var url = $"api/contexts?PageIndex={pageIndex}&PageSize={pageSize}&contextId={contextItemId}";

            var response = await _apiClient
       .GetAsync<LocalizerApiResponse<PaginatedItems<ContextModel>>>(url, ct);


            if (response?.Data?.Data is null || response?.Data?.Data is null)
                return new LocalizerApiResponse<PaginatedItems<ContextModel>>();

            var items = response?.Data.Data
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

    public async Task<LocalizerApiResponse<ContextModel>> PostContextAsync(ContextModel context, string userId, string targetLanguage, CancellationToken ct = default)
    {
        var request = new SaveContextRequest(context.ContextText, userId, targetLanguage);

        try
        {
            var result = await _apiClient.PostAsync<SaveContextRequest, LocalizerApiResponse<ContextModel>> ("api/contexts/saveContext",request);
            if (result.IsSuccess)
            {
                return result;
                //return new LocalizerApiResponse<ContextModel>
                //{
                //    ErrorMessage = result.ErrorMessage,
                //    ResponseMessage = result.ResponseMessage,
                //    IsSuccess = result.IsSuccess,
                //    Data = new ContextModel
                //    {
                //        Id = result.Data?.Id,
                //        ContextText = result.Data?.ContextText ?? string.Empty,
                //        CreatedAt = result.Data?.CreatedAt ?? DateTimeOffset.UtcNow
                //    }
                //};
            }
            else
            {
                return new LocalizerApiResponse<ContextModel>
                {
                    ErrorMessage = result.ErrorMessage,
                    ResponseMessage = result.ResponseMessage,
                    IsSuccess = result.IsSuccess,
                };
            }

        }
        catch (Exception ex)
        {
            return new LocalizerApiResponse<ContextModel>
            {
                ErrorMessage = ex.Message,
                ResponseMessage= ex.Message,
                IsSuccess = false,
            };
        }

    }

}
public record SaveContextRequest(string ContextText, string? createdByUserid, string targetLanguage);
//public class CreateContextResult
//{
//    public string? Id { get; set; }

//    public string ContextText { get; set; } = string.Empty;

//    public DateTimeOffset CreatedAt { get; set; }
//}