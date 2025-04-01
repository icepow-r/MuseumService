namespace MuseumService.Models;

// AuthModels.cs
public class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class AuthResponse
{
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
    public Employee Employee { get; set; }
}