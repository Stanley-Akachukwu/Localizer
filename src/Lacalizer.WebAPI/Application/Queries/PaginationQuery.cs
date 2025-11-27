namespace Lacalizer.WebAPI.Application.Queries;

public record PaginationQuery(int PageIndex = 1, int PageSize = 10);
