

namespace Lacalizer.Shared.Dtos;

public class CreateContextResult
{
    public string? Id { get; set; }

    public string ContextText { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }
}