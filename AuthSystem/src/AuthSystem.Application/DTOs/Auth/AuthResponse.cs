namespace AuthSystem.Application.DTOs.Auth;

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool RequiresTwoFactor { get; set; }
    public UserDto? User { get; set; }
}
