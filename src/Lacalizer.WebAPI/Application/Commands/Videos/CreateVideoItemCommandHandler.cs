using FluentValidation;
using Lacalizer.Shared.Dtos;
using Lacalizer.Shared.Enums;
using Lacalizer.WebAPI.Entites.Videos;
using Lacalizer.WebAPI.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NUlid;

namespace Lacalizer.WebAPI.Application.Commands.Videos;

public record CreateParticipatoryVideoItemCommand(
    string Language,
    string ContextText,
    string VideoUri, string TopicId) : IRequest<LocalizerApiResponse<CreateVideoItemResult>>;
public class CreateVideoItemResult
{
    public string Id { get; set; } = string.Empty;
    public string VideoUri { get; set; } = string.Empty;
}
public class CreateVideoItemCommandHandler : IRequestHandler<CreateParticipatoryVideoItemCommand, LocalizerApiResponse<CreateVideoItemResult>>
{
    private readonly LocalizeDbContext _db;
    private static readonly Ulid SystemUserId = Ulid.Empty;
    public CreateVideoItemCommandHandler(LocalizeDbContext db)
    {
        _db = db;
    }

    public async Task<LocalizerApiResponse<CreateVideoItemResult>> Handle(CreateParticipatoryVideoItemCommand request, CancellationToken ct)
    {
        var id = Ulid.NewUlid().ToString(); 

        var videoTopic = await _db.VideoContexts
                                  .FirstOrDefaultAsync(v => v.Id.Trim() == request.TopicId.Trim(), ct);

        if (videoTopic == null)
        {
            return new LocalizerApiResponse<CreateVideoItemResult>
            {
                IsSuccess = false,
                ErrorMessage = "Video topic not found.",
                StatusCode = 404
            };
        }

        var videoItem = new VideoItem
        {
            Id = id,
            VideoContextId = videoTopic.Id,
            Language = request.Language,
            ContextText = request.ContextText,
            VideoUri = request.VideoUri,
            Description = "Localized",
            IsActive = true,
            CreatedByUserId = SystemUserId.ToString(),
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,
            UpdatedByUserId = SystemUserId.ToString(),
            VideoType = VideoType.PARTICIPATION
        };

        await _db.VideoItems.AddAsync(videoItem, ct);
        await _db.SaveChangesAsync(ct);

        return new LocalizerApiResponse<CreateVideoItemResult>
        {
            IsSuccess = true,
            Data = new CreateVideoItemResult
            {
                Id = videoItem.Id,
                VideoUri = videoItem.VideoUri
            },
            StatusCode = 201,
            ResponseMessage = "Video item created successfully."
        };
    }
}


public class CreateVideoItemCommandValidator : AbstractValidator<CreateParticipatoryVideoItemCommand>
{
    public CreateVideoItemCommandValidator()
    {
        RuleFor(x => x.Language).NotEmpty();
        RuleFor(x => x.ContextText).NotEmpty();
        RuleFor(x => x.VideoUri).NotEmpty();
    }
}