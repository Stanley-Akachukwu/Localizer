using FluentValidation;
using Lacalizer.Shared.Dtos;
using Lacalizer.Shared.Enums;
using Lacalizer.WebAPI.Application.Commands.Videos;
using Lacalizer.WebAPI.Application.Queries;
using Lacalizer.WebAPI.Services.Validations;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace Lacalizer.WebAPI.Apis;

public static class VideoItemApi
{
    public static RouteGroupBuilder MapVideoItemAPI(this IEndpointRouteBuilder app)
    {
        var vApi = app.NewVersionedApi("videoitems");

        var v1 = vApi.MapGroup("/api/videoitems")
                      .WithTags("VideoItems");

        v1.MapGet("/", GetVideosAsync)
         .WithName("GetVideos")
         .WithSummary("Get paginated video items");

        v1.MapGet("/single", GetVideoByIdAsync)
                 .WithName("GetVideoById")
                 .WithSummary("Get single video by ID");

        v1.MapPost("/", CreateParticipatoryVideoItemAsync)
           .WithName("CreateParticipatoryVideoItem")
           .WithSummary("Create Particpatory video item")
           .WithDescription("Creates a new particpatory video item");

        v1.MapPut("/", UpdateVideoItemAsync)
           .WithName("UpdateVideoItem")
           .WithSummary("Update video item")
           .WithDescription("Updates an existing video item");

        v1.MapDelete("/", DeleteVideoItemAsync)
           .WithName("DeleteVideoItem")
           .WithSummary("Delete video item")
           .WithDescription("Deletes a video item by ID");

        return v1;
    }

    public static async Task<IResult> GetVideosAsync(
    [FromServices] IVideoItemQueries queries,
    [AsParameters] VideoPaginationQuery q,
    CancellationToken ct)
    {
        if (q.PageIndex <= 0 || q.PageSize <= 0)
            return TypedResults.BadRequest("Invalid pagination parameters.");

        var result = await queries.GetVideosAsync(q, ct);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> GetVideoByIdAsync(
     [FromServices] IVideoItemQueries queries,
     [AsParameters] GetVideoItemByIdRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Id))
            return TypedResults.BadRequest("VideoItem ID is required.");

        var result = await queries.GetVideoByIdAsync(request.Id, CancellationToken.None);
        return TypedResults.Ok(result);
    }


    private static Task<LocalizerApiResponse<CreateVideoItemResult>> CreateParticipatoryVideoItemAsync(
    CreateParticipatoryVideoItemCommand cmd,
    IValidator<CreateParticipatoryVideoItemCommand> validator,
    IValidationService validatorService,
    IMediator mediator)
    {
        return MediatorValidationHelper.ExecuteAsync<
            CreateParticipatoryVideoItemCommand,
            CreateVideoItemResult>(cmd, validator, validatorService, mediator);
    }

    private static Task<LocalizerApiResponse<UpdateVideoItemResult>> UpdateVideoItemAsync(
    UpdateVideoItemCommand cmd,
    IValidator<UpdateVideoItemCommand> validator,
    IValidationService validatorService,
    IMediator mediator)
    {
        return MediatorValidationHelper.ExecuteAsync<
            UpdateVideoItemCommand,
            UpdateVideoItemResult>(cmd, validator, validatorService, mediator);
    }

    private static async Task<IResult> DeleteVideoItemAsync(
        [AsParameters] GetVideoItemByIdRequest request,
        IMediator mediator)
    {
        if (string.IsNullOrWhiteSpace(request.Id))
            return TypedResults.BadRequest("VideoItem ID is required.");

        var cmd = new DeleteVideoItemCommand(request.Id);
        var result = await mediator.Send(cmd);
        return TypedResults.Ok(result);
    }
}

public record GetVideoItemByIdRequest(string Id);
public record VideoPaginationQuery(
    int PageIndex = 1,
    int PageSize = 10,
    string? Language = null,
    string? Title = null,
    VideoType VideoType = VideoType.TOPIC,
    DateTimeOffset? DateCreated = null,
    string? VideoTopicId = null
) : PaginationQuery(PageIndex, PageSize);