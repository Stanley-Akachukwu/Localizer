namespace Lacalizer.Mobile.Models;

public class UserModel
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

 
public class LoginResponse
{
    public bool Success { get; set; }

    public string? Token { get; set; }

    public DateTime? Expiration { get; set; }

    public string Message { get; set; } = string.Empty;

    public List<string> Errors { get; set; } = [];
}

public class RegisterResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = [];
}
public class AuthState
{
    public bool IsAuthenticated { get; set; }
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public string? Username { get; set; }
}