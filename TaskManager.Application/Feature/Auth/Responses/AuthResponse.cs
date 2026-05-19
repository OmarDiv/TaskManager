namespace TaskManager.Application.Feature.Auth.Responses
{
    public record AuthResponse(
     long Id,
     string? Email,
     string FirstName,
     string LastName,
     string Token,
     int ExpiresIn,
     string RefreshToken,
     DateTime RefreshTokenExpiration);
}