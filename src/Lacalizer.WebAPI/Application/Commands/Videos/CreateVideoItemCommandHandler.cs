using FluentValidation;
using Lacalizer.WebAPI.Dtos;
using Lacalizer.WebAPI.Entites;
using Lacalizer.WebAPI.Entites.Videos;
using Lacalizer.WebAPI.Infrastructure;
using MediatR;

namespace Lacalizer.WebAPI.Application.Commands.Videos;

public record CreateVideoItemCommand(
    string Language,
    string Title,
    string Topic,
    string BlobName) : IRequest<LocalizerApiResponse<CreateVideoItemResult>>;
public class CreateVideoItemResult
{
    public string Id { get; set; } = string.Empty;
    public string BlobName { get; set; } = string.Empty;
}
public class CreateVideoItemCommandHandler : IRequestHandler<CreateVideoItemCommand, LocalizerApiResponse<CreateVideoItemResult>>
{
    private readonly LocalizeContext _db;

    public CreateVideoItemCommandHandler(LocalizeContext db)
    {
        _db = db;
    }

    public async Task<LocalizerApiResponse<CreateVideoItemResult>> Handle(CreateVideoItemCommand request, CancellationToken cancellationToken)
    {
        var videoItem = new VideoItem
        {
            Language = request.Language,
            Title = request.Title,
            Topic = request.Topic,
            VideoUri = request.BlobName
        };

        await _db.VideoItems.AddAsync(videoItem, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return new LocalizerApiResponse<CreateVideoItemResult>
        {
            IsSuccess = true,
            Data = new CreateVideoItemResult
            {
                Id = videoItem.Id,
                BlobName = videoItem.VideoUri
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
        RuleFor(x => x.BlobName).NotEmpty();
    }
}