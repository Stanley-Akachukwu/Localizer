using Lacalizer.Shared.Dtos;
using Lacalizer.Shared.Enums;
using Lacalizer.WebAPI.Apis;
using Lacalizer.WebAPI.Entites.Videos;
using Lacalizer.WebAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Lacalizer.WebAPI.Application.Queries;

public class SingleVideoItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string VideoUri { get; set; } = string.Empty;
    public string? VideoTopicId { get; set; }
    public int SavedLikes { get; set; }
    public int SavedComments { get; set; }
    public int SavedShares { get; set; }
    public int SavedParticipants { get; set; }
}

public interface IVideoItemQueries
{
    Task<LocalizerApiResponse<PaginatedItems<SingleVideoItemDto>>> GetVideosAsync(
        VideoPaginationQuery query, CancellationToken ct);

    Task<LocalizerApiResponse<SingleVideoItemDto>> GetVideoByIdAsync(
        string videoId, CancellationToken ct);
}

public class VideoItemQueries : IVideoItemQueries
{
    private readonly LocalizeContext _dbContext;
    private readonly IMemoryCache _cache;

    public VideoItemQueries(LocalizeContext dbContext, IMemoryCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<LocalizerApiResponse<SingleVideoItemDto>> GetVideoByIdAsync(
        string videoId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(videoId))
            return LocalizerApiResponse<SingleVideoItemDto>.Failure("Invalid video ID.", StatusCodes.Status400BadRequest);

        var cacheKey = $"Video_{videoId}";

        if (_cache.TryGetValue(cacheKey, out SingleVideoItemDto cachedDto))
        {
            return LocalizerApiResponse<SingleVideoItemDto>.Success(cachedDto, StatusCodes.Status200OK);
        }

        try
        {
            var dto = await _dbContext.VideoItems
                .Where(v => v.Id == videoId)
                .Select(v => new SingleVideoItemDto
                {
                    Id = v.Id,
                    Language = v.Language,
                    Title = v.Title,
                    Topic = v.Topic,
                    VideoUri = v.VideoUri
                })
                .FirstOrDefaultAsync(ct);

            if (dto == null)
            {
                return LocalizerApiResponse<SingleVideoItemDto>.Failure(
                    "Video not found.",
                    StatusCodes.Status404NotFound);
            }

            _cache.Set(cacheKey, dto, TimeSpan.FromMinutes(5));

            return LocalizerApiResponse<SingleVideoItemDto>.Success(dto, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return LocalizerApiResponse<SingleVideoItemDto>.Failure(
                $"Internal server error: {ex.Message}",
                StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<LocalizerApiResponse<PaginatedItems<SingleVideoItemDto>>> GetVideosAsync(
        VideoPaginationQuery req, CancellationToken ct)
    {
        if (req.PageIndex <= 0 || req.PageSize <= 0)
            return LocalizerApiResponse<PaginatedItems<SingleVideoItemDto>>.Failure(
                "Invalid pagination parameters.",
                StatusCodes.Status400BadRequest);

        var cacheKey = $"Videos_{req.Language}_{req.Title}_{req.DateCreated?.ToString("yyyyMMdd")}_{req.PageIndex}_{req.PageSize}";

        if (_cache.TryGetValue(cacheKey, out PaginatedItems<SingleVideoItemDto> cachedItems) && req.VideoType != VideoType.PARTICIPATION)
        {
            return LocalizerApiResponse<PaginatedItems<SingleVideoItemDto>>.Success(cachedItems, StatusCodes.Status200OK);
        }

        try
        {
            IQueryable<VideoItem> query = _dbContext.VideoItems;

            if (!string.IsNullOrWhiteSpace(req.VideoTopicId) && req.VideoType==VideoType.PARTICIPATION)
                query = query.Where(v => v.VideoTopicId == req.VideoTopicId);

            if (!string.IsNullOrWhiteSpace(req.Language))
                query = query.Where(v => v.Language == req.Language);

            if (!string.IsNullOrWhiteSpace(req.Title))
                query = query.Where(v => v.Topic == req.Title);

            if (req.DateCreated.HasValue)
                query = query.Where(v => v.DateCreated.Value.Date == req.DateCreated.Value.Date);

            if (req.VideoType != VideoType.NONE)
                query = query.Where(v => v.VideoType == req.VideoType);

            var totalCount = await query.CountAsync(ct);

            if (totalCount == 0)
            {
                return LocalizerApiResponse<PaginatedItems<SingleVideoItemDto>>.Failure(
                    "No videos found.",
                    StatusCodes.Status404NotFound);
            }

            var items = await query
                .OrderBy(v => v.Title)
                .Skip((req.PageIndex - 1) * req.PageSize)
                .Take(req.PageSize)
                .Select(v => new SingleVideoItemDto
                {
                    Id = v.Id,
                    Language = v.Language,
                    Title = v.Title,
                    Topic = v.Topic,
                    VideoTopicId = v.VideoTopicId,
                    VideoUri = v.VideoUri, //= v.VideoType == VideoType.TOPIC? v.VideoUri: "https://github.com/ewerspej/maui-samples/blob/main/assets/bigbuckbunny.mp4?raw=true",
                    SavedLikes = v.LikeCounts,
                    SavedComments =v.CommentCounts,
                    SavedShares =v.ShareCounts,
                    SavedParticipants = v.ParticipantCounts
                })
                .ToListAsync(ct);

            var paginatedResult = new PaginatedItems<SingleVideoItemDto>(
                req.PageIndex,
                req.PageSize,
                totalCount,
                items);

            _cache.Set(cacheKey, paginatedResult, TimeSpan.FromMinutes(5));

            return LocalizerApiResponse<PaginatedItems<SingleVideoItemDto>>.Success(paginatedResult, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return LocalizerApiResponse<PaginatedItems<SingleVideoItemDto>>.Failure(
                $"Internal server error: {ex.Message}",
                StatusCodes.Status500InternalServerError);
        }
    }
}







