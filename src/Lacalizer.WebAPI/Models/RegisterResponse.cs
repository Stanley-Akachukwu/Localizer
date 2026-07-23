namespace Lacalizer.WebAPI.Models;

public class RegisterResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = [];
}
