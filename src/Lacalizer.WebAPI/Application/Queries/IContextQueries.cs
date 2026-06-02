using Lacalizer.Shared.Dtos;
using Lacalizer.WebAPI.Apis;
using Lacalizer.WebAPI.Entites.Contexts;
using Lacalizer.WebAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Lacalizer.WebAPI.Application.Queries;
 

public class SingleContextDto
{
    public string Id { get; set; } = string.Empty;
    public string CreatedByUserId { get; set; } = string.Empty;
    public string ContextText { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}

public interface IContextQueries
{
    Task<LocalizerApiResponse<PaginatedItems<SingleContextDto>>> GetContextsAsync(
        ContextPaginationQuery query, CancellationToken ct);

    Task<LocalizerApiResponse<SingleContextDto>> GetContextsByIdAsync(
        string contextItemId, CancellationToken ct);
}

public class ContextQueries : IContextQueries
{
    private readonly LocalizeDbContext _dbContext;
    private readonly IMemoryCache _cache;

    public ContextQueries(LocalizeDbContext dbContext, IMemoryCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<LocalizerApiResponse<SingleContextDto>> GetContextsByIdAsync(
        string contextItemId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(contextItemId))
            return LocalizerApiResponse<SingleContextDto>.Failure("Invalid video ID.", StatusCodes.Status400BadRequest);

        var cacheKey = $"Context_{contextItemId}";

        if (_cache.TryGetValue(cacheKey, out SingleContextDto cachedDto))
        {
            return LocalizerApiResponse<SingleContextDto>.Success(cachedDto, StatusCodes.Status200OK);
        }

        try
        {
            var dto = await _dbContext.LocalizeContexts
                .Where(v => v.Id == contextItemId)
                .Select(v => new SingleContextDto
                {
                    Id = v.Id,
                    ContextText = v.ContextText,
                   CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = v.CreatedByUserId!
                })
                .FirstOrDefaultAsync(ct);

            if (dto == null)
            {
                return LocalizerApiResponse<SingleContextDto>.Failure(
                    "Video not found.",
                    StatusCodes.Status404NotFound);
            }

            return LocalizerApiResponse<SingleContextDto>.Success(dto, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return LocalizerApiResponse<SingleContextDto>.Failure(
                $"Internal server error: {ex.Message}",
                StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<LocalizerApiResponse<PaginatedItems<SingleContextDto>>> GetContextsAsync(
        ContextPaginationQuery req, CancellationToken ct)
    {
        if (req.PageIndex <= 0 || req.PageSize <= 0)
            return LocalizerApiResponse<PaginatedItems<SingleContextDto>>.Failure(
                "Invalid pagination parameters.",
                StatusCodes.Status400BadRequest);
        try
        {
            IQueryable<LocalizeContext> query = _dbContext.LocalizeContexts.Where(c => c.Id == req.contextItemId);

            var totalCount = await query.CountAsync(ct);

            if (totalCount == 0)
            {
                return LocalizerApiResponse<PaginatedItems<SingleContextDto>>.Failure(
                    "No videos found.",
                    StatusCodes.Status404NotFound);
            }

            var items = await query
                .OrderBy(v => v.DateCreated)
                .Skip((req.PageIndex - 1) * req.PageSize)
                .Take(req.PageSize)
                .Select(v => new SingleContextDto
                {
                    Id = v.Id,
                    ContextText = v.ContextText,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = v.CreatedByUserId!
                })
                .ToListAsync(ct);

            var paginatedResult = new PaginatedItems<SingleContextDto>(
                req.PageIndex,
                req.PageSize,
                totalCount,
                items);


            return LocalizerApiResponse<PaginatedItems<SingleContextDto>>.Success(paginatedResult, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return LocalizerApiResponse<PaginatedItems<SingleContextDto>>.Failure(
                $"Internal server error: {ex.Message}",
                StatusCodes.Status500InternalServerError);
        }
    }
}

