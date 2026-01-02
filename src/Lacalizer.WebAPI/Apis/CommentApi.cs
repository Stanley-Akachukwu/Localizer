using FluentValidation;
using Lacalizer.Shared.Dtos;
using Lacalizer.WebAPI.Application.Commands.Videos;
using Lacalizer.WebAPI.Application.Queries;
using Lacalizer.WebAPI.Services.Validations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Lacalizer.WebAPI.Apis;

 
public static class CommentApi
{
    public static RouteGroupBuilder MapCommentAPI(this IEndpointRouteBuilder app)
    {
        var vApi = app.NewVersionedApi("comments");

        var v1 = vApi.MapGroup("/api/comments")
                      .WithTags("Comments");

        v1.MapGet("/", GetCommentsAsync)
         .WithName("GetComments")
         .WithSummary("Get paginated commets");

        v1.MapPost("/saveComment", SaveCommentAsync)
           .WithName("SaveComment")
           .WithSummary("Save Comment")
           .WithDescription("Save comment for video item");

        return v1;
    }

    public static async Task<IResult> GetCommentsAsync(
    [FromServices] ICommentQueries queries,
    [AsParameters] CommentPaginationQuery q,
    CancellationToken ct)
    {
        if (q.PageIndex <= 0 || q.PageSize <= 0)
            return TypedResults.BadRequest("Invalid pagination parameters.");

        var result = await queries.GetCommentsAsync(q, ct);
        return TypedResults.Ok(result);
    }

   
    private static Task<LocalizerApiResponse<CreateCommentResult>> SaveCommentAsync(
       SaveCommentCommand cmd,
       IValidator<SaveCommentCommand> validator,
       IValidationService validatorService,
       IMediator mediator)
    {
        return MediatorValidationHelper.ExecuteAsync<
            SaveCommentCommand,
            CreateCommentResult>(cmd, validator, validatorService, mediator);
    }
}



public record CommentPaginationQuery(
    int PageIndex = 1,
    int PageSize = 10,
   string? VideoItemId = null,
    DateTimeOffset? DateCreated = null
) : PaginationQuery(PageIndex, PageSize);
