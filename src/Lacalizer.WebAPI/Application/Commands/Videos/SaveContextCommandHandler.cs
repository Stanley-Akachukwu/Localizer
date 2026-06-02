using FluentValidation;
using Lacalizer.Shared.Dtos;
using Lacalizer.WebAPI.Entites.Contexts;
using Lacalizer.WebAPI.Infrastructure;
using MediatR;
using NUlid;

namespace Lacalizer.WebAPI.Application.Commands.Videos;

 

public record SaveContextCommand(string ContextText, string? createdByUserid) : IRequest<LocalizerApiResponse<CreateContextResult>>;

public class SaveContextCommandHandler : IRequestHandler<SaveContextCommand, LocalizerApiResponse<CreateContextResult>>
{
    private readonly LocalizeDbContext _db;
    private static readonly Ulid SystemUserId = Ulid.Empty;
    public SaveContextCommandHandler(LocalizeDbContext db)
    {
        _db = db;
    }

    public async Task<LocalizerApiResponse<CreateContextResult>> Handle(SaveContextCommand request, CancellationToken ct)
    {
        var id = Ulid.NewUlid().ToString();
        var context = new LocalizeContext
        {
            Id = id,
            ContextText = request.ContextText,
            Description = $"Description - {request.ContextText}",
            IsActive = true,
            CreatedByUserId = request.createdByUserid!.ToString(),
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,
            UpdatedByUserId = SystemUserId.ToString(),
            UID = id,
        };

        await _db.LocalizeContexts.AddAsync(context, ct);
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
            ResponseMessage = "Video comment added successfully."
        };
    }
}


public class SaveContextCommandValidator : AbstractValidator<SaveContextCommand>
{
    public SaveContextCommandValidator()
    {
        RuleFor(x => x.ContextText).NotEmpty();
        RuleFor(x => x.createdByUserid).NotEmpty();
    }
}

