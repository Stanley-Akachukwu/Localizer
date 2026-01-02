using Lacalizer.Shared.Dtos;
using Lacalizer.WebAPI.Apis;
using Lacalizer.WebAPI.Entites;
using Lacalizer.WebAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;


namespace Lacalizer.WebAPI.Application.Queries;
public class SingleCommentDto
{
    public string Id { get; set; } = string.Empty;
    public string VideoItemId { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ParentId { get; set; } = string.Empty;
}

public interface ICommentQueries
{
    Task<LocalizerApiResponse<PaginatedItems<SingleCommentDto>>> GetCommentsAsync(
        CommentPaginationQuery query, CancellationToken ct);

    Task<LocalizerApiResponse<SingleCommentDto>> GetCommentsByIdAsync(
        string videoId, CancellationToken ct);
}

public class CommentQueries : ICommentQueries
{
    private readonly LocalizeContext _dbContext;
    private readonly IMemoryCache _cache;

    public CommentQueries(LocalizeContext dbContext, IMemoryCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<LocalizerApiResponse<SingleCommentDto>> GetCommentsByIdAsync(
        string videoId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(videoId))
            return LocalizerApiResponse<SingleCommentDto>.Failure("Invalid video ID.", StatusCodes.Status400BadRequest);

        var cacheKey = $"Comment_{videoId}";

        if (_cache.TryGetValue(cacheKey, out SingleCommentDto cachedDto))
        {
            return LocalizerApiResponse<SingleCommentDto>.Success(cachedDto, StatusCodes.Status200OK);
        }

        try
        {
            var dto = await _dbContext.Comments
                .Where(v => v.VideoItemId == videoId)
                .Select(v => new SingleCommentDto
                {
                    Id = v.Id,
                    VideoItemId = v.Id,
                    Author = v.Author, 
                    Content = v.Content, 
                    ParentId = v.ParentId 
                })
                .FirstOrDefaultAsync(ct);

            if (dto == null)
            {
                return LocalizerApiResponse<SingleCommentDto>.Failure(
                    "Video not found.",
                    StatusCodes.Status404NotFound);
            }

            return LocalizerApiResponse<SingleCommentDto>.Success(dto, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return LocalizerApiResponse<SingleCommentDto>.Failure(
                $"Internal server error: {ex.Message}",
                StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<LocalizerApiResponse<PaginatedItems<SingleCommentDto>>> GetCommentsAsync(
        CommentPaginationQuery req, CancellationToken ct)
    {
        if (req.PageIndex <= 0 || req.PageSize <= 0)
            return LocalizerApiResponse<PaginatedItems<SingleCommentDto>>.Failure(
                "Invalid pagination parameters.",
                StatusCodes.Status400BadRequest);
        try
        {
            IQueryable<Comment> query = _dbContext.Comments.Where(c=>c.VideoItemId == req.VideoItemId);

            var totalCount = await query.CountAsync(ct);

            if (totalCount == 0)
            {
                return LocalizerApiResponse<PaginatedItems<SingleCommentDto>>.Failure(
                    "No videos found.",
                    StatusCodes.Status404NotFound);
            }

            var items = await query
                .OrderBy(v => v.DateCreated)
                .Skip((req.PageIndex - 1) * req.PageSize)
                .Take(req.PageSize)
                .Select(v => new SingleCommentDto
                {
                    Id = v.Id,
                    VideoItemId = v.Id,
                    Author = v.Author,
                    Content = v.Content,
                    ParentId = v.ParentId
                })
                .ToListAsync(ct);

            var paginatedResult = new PaginatedItems<SingleCommentDto>(
                req.PageIndex,
                req.PageSize,
                totalCount,
                items);


            return LocalizerApiResponse<PaginatedItems<SingleCommentDto>>.Success(paginatedResult, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return LocalizerApiResponse<PaginatedItems<SingleCommentDto>>.Failure(
                $"Internal server error: {ex.Message}",
                StatusCodes.Status500InternalServerError);
        }
    }
}
