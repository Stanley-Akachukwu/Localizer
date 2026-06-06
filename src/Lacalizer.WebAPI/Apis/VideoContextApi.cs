using FluentValidation;
using Lacalizer.Shared.Dtos;
using Lacalizer.WebAPI.Application.Commands.Videos;
using Lacalizer.WebAPI.Application.Queries;
using Lacalizer.WebAPI.Services.Validations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Lacalizer.WebAPI.Apis;

public static class VideoContextApi
{
    public static RouteGroupBuilder MapVideoContextAPI(this IEndpointRouteBuilder app)
    {
        var vApi = app.NewVersionedApi("contexts");

        var v1 = vApi.MapGroup("/api/contexts")
                      .WithTags("Contexts");

        v1.MapGet("/", GetContextsAsync)
         .WithName("GetContexts")
         .WithSummary("Get paginated contexts");

        v1.MapPost("/saveContext", SaveContextAsync)
           .WithName("SaveContext")
           .WithSummary("Save Context")
           .WithDescription("Save context item");

        return v1;
    }

    public static async Task<IResult> GetContextsAsync(
    [FromServices] IContextQueries queries,
    [AsParameters] ContextPaginationQuery q,
    CancellationToken ct)
    {
        if (q.PageIndex <= 0 || q.PageSize <= 0)
            return TypedResults.BadRequest("Invalid pagination parameters.");

        var result = await queries.GetContextsAsync(q, ct);
        return TypedResults.Ok(result);
    }


    private static Task<LocalizerApiResponse<CreateContextResult>> SaveContextAsync(
       SaveContextCommand cmd,
       IValidator<SaveContextCommand> validator,
       IValidationService validatorService,
       IMediator mediator)
    {
        return MediatorValidationHelper.ExecuteAsync<
            SaveContextCommand,
            CreateContextResult>(cmd, validator, validatorService, mediator);
    }
}



public record ContextPaginationQuery(
    int PageIndex = 1,
    int PageSize = 10,
   string? contextItemId = null,
    DateTimeOffset? DateCreated = null
) : PaginationQuery(PageIndex, PageSize);
