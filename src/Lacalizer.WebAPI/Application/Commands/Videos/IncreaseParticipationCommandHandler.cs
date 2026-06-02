using FluentValidation;
using Lacalizer.Shared.Dtos;
using Lacalizer.WebAPI.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lacalizer.WebAPI.Application.Commands.Videos;
 

public record IncreaseParticipationCommand(
    int Likes,
    string VideoItemId) : IRequest<LocalizerApiResponse<int>>;

public class IncreaseParticipationCommandHandler : IRequestHandler<IncreaseParticipationCommand, LocalizerApiResponse<int>>
{
    private readonly LocalizeDbContext _db;
    public IncreaseParticipationCommandHandler(LocalizeDbContext db)
    {
        _db = db;
    }

    public async Task<LocalizerApiResponse<int>> Handle(IncreaseParticipationCommand request, CancellationToken ct)
    {
        var videoItem = await _db.VideoItems
                                  .FirstOrDefaultAsync(v => v.Id.Trim() == request.VideoItemId.Trim(), ct);

        if (videoItem == null)
        {
            return new LocalizerApiResponse<int>
            {
                IsSuccess = false,
                ErrorMessage = "Video item not found.",
                StatusCode = 404,
                Data = request.Likes
            };
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
}


public class IncreaseParticipationCommandValidator : AbstractValidator<IncreaseParticipationCommand>
{
    public IncreaseParticipationCommandValidator()
    {
        RuleFor(x => x.VideoItemId).NotEmpty();
    }
}
