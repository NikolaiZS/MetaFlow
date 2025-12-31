using System.Text.Json.Serialization;

namespace MetaFlow.Contracts.Users
{
    public record AuthResponse(    
    [property: JsonPropertyName("userId")] Guid UserId,
        [property: JsonPropertyName("email")] string Email,
        [property: JsonPropertyName("username")] string Username,
        [property: JsonPropertyName("fullName")] string? FullName,
        [property: JsonPropertyName("token")] string Token,
        [property: JsonPropertyName("expiresAt")] DateTime ExpiresAt
    /*
    Guid UserId,
    string Email,
    string Username,
    string? FullName,
    string Token,
    DateTime ExpiresAt
    */
    );
}