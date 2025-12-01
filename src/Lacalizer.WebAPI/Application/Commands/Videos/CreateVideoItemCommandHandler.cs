using FluentValidation;
using Lacalizer.Shared.Dtos;
using Lacalizer.Shared.Enums;
using Lacalizer.WebAPI.Entites.Videos;
using Lacalizer.WebAPI.Infrastructure;
using MediatR;
using NUlid;

namespace Lacalizer.WebAPI.Application.Commands.Videos;

public record CreateVideoItemCommand(
    string Language,
    string Title,
    string Topic,
    string VideoUri) : IRequest<LocalizerApiResponse<CreateVideoItemResult>>;
public class CreateVideoItemResult
{
    public string Id { get; set; } = string.Empty;
    public string VideoUri { get; set; } = string.Empty;
}
public class CreateVideoItemCommandHandler : IRequestHandler<CreateVideoItemCommand, LocalizerApiResponse<CreateVideoItemResult>>
{
    private readonly LocalizeContext _db;
    private static readonly Ulid SystemUserId = Ulid.Empty;
    public CreateVideoItemCommandHandler(LocalizeContext db)
    {
        _db = db;
    }

    public async Task<LocalizerApiResponse<CreateVideoItemResult>> Handle(CreateVideoItemCommand request, CancellationToken cancellationToken)
    {
        var id = Ulid.NewUlid().ToString();
        var videoItem = new VideoItem
        {
            Id = id,
            Language = request.Language,
            Title = request.Title,
            Topic = request.Topic,
            VideoUri = request.VideoUri,
            Description = request.Title,
            IsActive = true,
            CreatedByUserId = SystemUserId.ToString(),
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,
            UpdatedByUserId = SystemUserId.ToString(),
            UID = id,
            VideoType = VideoType.PARTICIPATION
        };

        await _db.VideoItems.AddAsync(videoItem, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

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


public class CreateVideoItemCommandValidator : AbstractValidator<CreateVideoItemCommand>
{
    public CreateVideoItemCommandValidator()
    {
        RuleFor(x => x.Language).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Topic).NotEmpty();
        RuleFor(x => x.VideoUri).NotEmpty();
    }
}