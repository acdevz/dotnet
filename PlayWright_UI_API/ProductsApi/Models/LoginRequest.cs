namespace ProductApi.Models;

public class LoginRequest
{
    public string Username { get; set; } = String.Empty;
    public string Password { get; set; } = String.Empty;
}