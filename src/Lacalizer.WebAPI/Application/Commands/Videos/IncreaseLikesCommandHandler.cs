using FluentValidation;
using Lacalizer.Shared.Dtos;
using Lacalizer.WebAPI.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lacalizer.WebAPI.Application.Commands.Videos;

public record IncreaseLikesCommand(string VideoItemId) : IRequest<LocalizerApiResponse<int>>;
 
public class IncreaseLikesCommandHandler : IRequestHandler<IncreaseLikesCommand, LocalizerApiResponse<int>>
{
    private readonly LocalizeDbContext _db;
    public IncreaseLikesCommandHandler(LocalizeDbContext db)
    {
        _db = db;
    }

    public async Task<LocalizerApiResponse<int>> Handle(IncreaseLikesCommand request, CancellationToken ct)
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
                Data =0
            };
        }
        videoItem.LikeCounts++;
        _db.VideoItems.Update(videoItem);
        await _db.SaveChangesAsync(ct);

        return new LocalizerApiResponse<int>
        {
            IsSuccess = true,
            Data = videoItem.LikeCounts++,
            StatusCode = 201,
            ResponseMessage = "Video item likes increased successfully."
        };
    }
}


public class IncreaseLikesCommandValidator : AbstractValidator<IncreaseLikesCommand>
{
    public IncreaseLikesCommandValidator()
    {
        RuleFor(x => x.VideoItemId).NotEmpty();
    }
}