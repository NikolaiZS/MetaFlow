namespace EmailService.Api;

public class RateLimitExceededException : Exception
{
    public RateLimitExceededException(string message) : base(message) { }
}

public record EmailTemplateResponse(
    string Name,
    string Description,
    Dictionary<string, string> Variables);

public record EmailSendResponse(
    bool Success,
    string? ErrorMessage = null,
    DateTime? SentAt = null);
