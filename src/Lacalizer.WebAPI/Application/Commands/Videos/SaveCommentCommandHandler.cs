using FluentValidation;
using Lacalizer.Shared.Dtos;
using Lacalizer.WebAPI.Entites;
using Lacalizer.WebAPI.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NUlid;

namespace Lacalizer.WebAPI.Application.Commands.Videos;

 

public record SaveCommentCommand(string VideoItemId, string? ParentId, string Author, string Content) : IRequest<LocalizerApiResponse<CreateCommentResult>>;

public class SaveCommentCommandHandler : IRequestHandler<SaveCommentCommand, LocalizerApiResponse<CreateCommentResult>>
{
    private readonly LocalizeDbContext _db;
    private static readonly Ulid SystemUserId = Ulid.Empty;
    public SaveCommentCommandHandler(LocalizeDbContext db)
    {
        _db = db;
    }

    public async Task<LocalizerApiResponse<CreateCommentResult>> Handle(SaveCommentCommand request, CancellationToken ct)
    {
        var id = Ulid.NewUlid().ToString();
        var videoItem = await _db.VideoItems
                                  .FirstOrDefaultAsync(v => v.Id.Trim() == request.VideoItemId.Trim(), ct);

        if (videoItem == null)
        {
            return new LocalizerApiResponse<CreateCommentResult>
            {
                IsSuccess = false,
                ErrorMessage = "Video item not found.",
                StatusCode = 404,
                Data = null
            };
        }

        var comment = new Comment
        {
            Id = id,
            VideoItemId = request.VideoItemId,
            ParentId = string.IsNullOrWhiteSpace(request.ParentId) ? null : request.ParentId,
            Author = request.Author,
            ContextText = request.Content,
            Description = request.Content,
            IsActive = true,
            CreatedByUserId = SystemUserId.ToString(),
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,
            UpdatedByUserId = SystemUserId.ToString(),
        };

        await _db.Comments.AddAsync(comment, ct);
        videoItem.CommentCounts++;
        _db.VideoItems.Update(videoItem);
        await _db.SaveChangesAsync(ct);

        return new LocalizerApiResponse<CreateCommentResult>
        {
            IsSuccess = true,
            Data = new CreateCommentResult
            {
                CommentId = comment.Id,
                VideoItemId = videoItem.Id,
                CommentCount = videoItem.CommentCounts
            },
            StatusCode = 201,
            ResponseMessage = "Video comment added successfully."
        };
    }
}


public class SaveCommentCommandValidator : AbstractValidator<SaveCommentCommand>
{
    public SaveCommentCommandValidator()
    {
        RuleFor(x => x.VideoItemId).NotEmpty();
        RuleFor(x => x.Author).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Content).NotEmpty().MaximumLength(1000);
    }
}
