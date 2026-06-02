
namespace Lacalizer.Mobile.Models;

public class ContextModel
{
    public string? Id { get; set; }

    public string ContextText { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }
}