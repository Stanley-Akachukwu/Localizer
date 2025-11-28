namespace Lacalizer.Shared.Dtos;


public class LocalizerApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? ResponseMessage { get; set; }
    public string? ErrorMessage { get; set; }
    public object? Errors { get; set; }
    public int StatusCode { get; set; }

    public static LocalizerApiResponse<T> Success(T data, int statusCode) =>
        new()
        {
            IsSuccess = true,
            Data = data,
            StatusCode = statusCode
        };

    public static LocalizerApiResponse<T> Failure(string errorMessage, int statusCode) =>
        new()
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            StatusCode = statusCode
        };

    public static LocalizerApiResponse<T> Failure(object errors, int statusCode) =>
        new()
        {
            IsSuccess = false,
            Errors = errors,
            StatusCode = statusCode
        };
}

public class PaginatedItems<TEntity>(int pageIndex, int pageSize, long count, IEnumerable<TEntity> data) where TEntity : class
{
    public int PageIndex { get; } = pageIndex;

    public int PageSize { get; } = pageSize;

    public long Count { get; } = count;

    public IEnumerable<TEntity> Data { get; } = data;
}
