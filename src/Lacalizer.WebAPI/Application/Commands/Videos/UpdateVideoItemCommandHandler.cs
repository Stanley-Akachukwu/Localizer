using FluentValidation;
using Lacalizer.Shared.Dtos;
using Lacalizer.WebAPI.Infrastructure;
using MediatR;

namespace Lacalizer.WebAPI.Application.Commands.Videos;

public record UpdateVideoItemCommand(
    string Id,
    string Language,
    string ContextText,
    string BlobName) : IRequest<LocalizerApiResponse<UpdateVideoItemResult>>;
public class UpdateVideoItemResult
{
    public string Id { get; set; } = string.Empty;
    public string BlobName { get; set; } = string.Empty;
}
public class UpdateVideoItemCommandHandler : IRequestHandler<UpdateVideoItemCommand, LocalizerApiResponse<UpdateVideoItemResult>>
{
    private readonly LocalizeDbContext _db;

    public UpdateVideoItemCommandHandler(LocalizeDbContext db)
    {
        _db = db;
    }

    public async Task<LocalizerApiResponse<UpdateVideoItemResult>> Handle(UpdateVideoItemCommand request, CancellationToken cancellationToken)
    {
        var existing = await _db.VideoItems.FindAsync(new object[] { request.Id }, cancellationToken);
        if (existing == null)
        {
            return LocalizerApiResponse<UpdateVideoItemResult>.Failure($"Video item with ID {request.Id} not found.", StatusCodes.Status404NotFound);
        }

        existing.Language = request.Language;
        existing.ContextText = request.ContextText;
        existing.VideoUri = request.BlobName;

        _db.VideoItems.Update(existing);
        await _db.SaveChangesAsync(cancellationToken);

        return new LocalizerApiResponse<UpdateVideoItemResult>
        {
            IsSuccess = true,
            Data = new UpdateVideoItemResult
            {
                Id = existing.Id,
                BlobName = existing.VideoUri
            },
            StatusCode = 201,
            ResponseMessage = "Video item updated successfully."
        };
    }
}

public class UpdateVideoItemCommandValidator : AbstractValidator<UpdateVideoItemCommand>
{
    public UpdateVideoItemCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Language).NotEmpty();
        RuleFor(x => x.ContextText).NotEmpty();
        RuleFor(x => x.BlobName).NotEmpty();
    }
}