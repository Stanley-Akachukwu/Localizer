using FluentValidation;
using Lacalizer.Shared.Dtos;
using Lacalizer.Shared.Enums;
using Lacalizer.WebAPI.Entites.Videos;
using Lacalizer.WebAPI.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NUlid;

namespace Lacalizer.WebAPI.Application.Commands.Videos;
 

public record IncreaseParticipationCommand(
    int Likes,
    string VideoItemId, string VideoContextId, string CreatedByUserId, string VideoUri) : IRequest<LocalizerApiResponse<int>>;

public class IncreaseParticipationCommandHandler : IRequestHandler<IncreaseParticipationCommand, LocalizerApiResponse<int>>
{
    private readonly LocalizeDbContext _db;
    public IncreaseParticipationCommandHandler(LocalizeDbContext db)
    {
        _db = db;
    }

    public async Task<LocalizerApiResponse<int>> Handle(IncreaseParticipationCommand request, CancellationToken ct)
    {

        var videoContext = await _db.VideoContexts
                                .FirstOrDefaultAsync(v => v.Id.Trim() == request.VideoContextId.Trim(), ct);
        var videoItem = await _db.VideoItems
                                  .FirstOrDefaultAsync(v => v.Id.Trim() == request.VideoItemId.Trim(), ct);

        if (videoItem == null && videoContext!=null)
        {
            videoItem= await CreateVideoItem(videoContext, request, ct);
        }
        videoItem.ParticipantCounts++;
        _db.VideoItems.Update(videoItem);
        await _db.SaveChangesAsync(ct);

        return new LocalizerApiResponse<int>
        {
            IsSuccess = true,
            Data = videoItem.LikeCounts++,
            StatusCode = 201,
            ResponseMessage = "Video item localizing participation increased successfully."
        };
    }

    public async Task<VideoItem?> CreateVideoItem(
     VideoContext? videoContext,
     IncreaseParticipationCommand request,
     CancellationToken ct)
    {
        if (videoContext is null)
            return null;

        var now = DateTime.UtcNow;

        var videoItem = new VideoItem
        {
            Id = Ulid.NewUlid().ToString(),
            VideoContextId = videoContext.Id,
            Language = videoContext.TargetLanguage,
            ContextText = videoContext.ContextText,
            VideoUri = request.VideoUri!,
            Description = "Localized",
            IsActive = true,
            CreatedByUserId = request.CreatedByUserId,
            UpdatedByUserId = request.CreatedByUserId,
            DateCreated = now,
            DateUpdated = now,
            VideoType = VideoType.CONTEXT
        };

        await _db.VideoItems.AddAsync(videoItem, ct);
        await _db.SaveChangesAsync(ct);

        return videoItem;
    }
}


public class IncreaseParticipationCommandValidator : AbstractValidator<IncreaseParticipationCommand>
{
    public IncreaseParticipationCommandValidator()
    {
        RuleFor(x => x.VideoItemId).NotEmpty();
        RuleFor(x => x.VideoContextId).NotEmpty();
    }
}
