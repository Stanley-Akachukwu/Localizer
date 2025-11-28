using Microsoft.EntityFrameworkCore;
using Lacalizer.WebAPI.Infrastructure;
using MediatR;
using Lacalizer.Shared.Dtos;

namespace Lacalizer.WebAPI.Application.Commands.Videos;

public record DeleteVideoItemCommand(string Id) : IRequest<LocalizerApiResponse<DeleteVideoItemResult>>;

public record DeleteVideoItemResult(string Id);

public class DeleteVideoItemCommandHandler
    : IRequestHandler<DeleteVideoItemCommand, LocalizerApiResponse<DeleteVideoItemResult>>
{
    private readonly LocalizeContext _db;
    public DeleteVideoItemCommandHandler(
        LocalizeContext db)
    {
        _db = db;
    }

    public async Task<LocalizerApiResponse<DeleteVideoItemResult>> Handle(
        DeleteVideoItemCommand request,CancellationToken ct)
    {
        var video = await _db.VideoItems
            .FirstOrDefaultAsync(v => v.Id == request.Id, ct);

        if (video == null)
        {
            return LocalizerApiResponse<DeleteVideoItemResult>
                .Failure("Video item not found.", StatusCodes.Status404NotFound);
        }
        _db.VideoItems.Remove(video);

        await _db.SaveChangesAsync(ct);


        return LocalizerApiResponse<DeleteVideoItemResult>
            .Success(new DeleteVideoItemResult(video.Id),
                     StatusCodes.Status200OK);
    }
}
