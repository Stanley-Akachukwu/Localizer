using FluentValidation;
using Lacalizer.Shared.Dtos;
using Lacalizer.WebAPI.Entites.Videos;
using Lacalizer.WebAPI.Infrastructure;
using MediatR;
using NUlid;

namespace Lacalizer.WebAPI.Application.Commands.Videos;

 

public record SaveContextCommand(string ContextText, string? createdByUserid, string targetLanguage) : IRequest<LocalizerApiResponse<CreateContextResult>>;

public class SaveContextCommandHandler : IRequestHandler<SaveContextCommand, LocalizerApiResponse<CreateContextResult>>
{
    private readonly LocalizeDbContext _db;
    public SaveContextCommandHandler(LocalizeDbContext db)
    {
        _db = db;
    }

    public async Task<LocalizerApiResponse<CreateContextResult>> Handle(SaveContextCommand request, CancellationToken ct)
    {
        try
        {
            var context = new VideoContext
            {
                Id = Ulid.NewUlid().ToString(),
                ContextText = request.ContextText,
                Description = $"Actual text for localization",
                IsActive = true,
                CreatedByUserId = request.createdByUserid!.ToString(),
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                UserId = request.createdByUserid,
                TargetLanguage = request.targetLanguage,
            };

            await _db.VideoContexts.AddAsync(context, ct);
            await _db.SaveChangesAsync(ct);

            return new LocalizerApiResponse<CreateContextResult>
            {
                IsSuccess = true,
                Data = new CreateContextResult
                {
                    Id = context.Id,
                    ContextText = context.ContextText,
                    CreatedAt = context.DateCreated.Value
                },
                StatusCode = 201,
                ResponseMessage = "Video context added successfully."
            };
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}


public class SaveContextCommandValidator : AbstractValidator<SaveContextCommand>
{
    public SaveContextCommandValidator()
    {
        RuleFor(x => x.ContextText).NotEmpty();
        RuleFor(x => x.createdByUserid).NotEmpty();
        RuleFor(x => x.targetLanguage).NotEmpty();
    }
}

