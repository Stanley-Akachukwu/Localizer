namespace Lacalizer.WebAPI.Models;

public class LoginResponse
{
    public bool Success { get; set; }

    public string? Token { get; set; }

    public DateTime? Expiration { get; set; }

    public string Message { get; set; } = string.Empty;

    public List<string> Errors { get; set; } = [];
}