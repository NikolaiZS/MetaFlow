using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace MetaFlow.Infrastructure.Services.Email;

/// <summary>
/// Клиент для взаимодействия с Email Service микросервисом
/// </summary>
public class EmailServiceClient : IEmailServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EmailServiceClient> _logger;

    public EmailServiceClient(HttpClient httpClient, ILogger<EmailServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(
        string to,
        string subject,
        string template,
        Dictionary<string, string> variables,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new EmailServiceRequest(
                To: to,
                Subject: subject,
                Template: template,
                Variables: variables);

            var response = await _httpClient.PostAsJsonAsync(
                "/api/email/send",
                request,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<EmailServiceResponse>(cancellationToken: cancellationToken);
            return result?.Success ?? false;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}. Email service request failed", to);
            return false;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout sending email to {To}", to);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error sending email to {To}", to);
            return false;
        }
    }

    public async Task<bool> SendBatchEmailsAsync(
        IEnumerable<EmailServiceRequest> requests,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                "/api/email/send-batch",
                requests,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<EmailServiceResponse>(cancellationToken: cancellationToken);
            return result?.Success ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send batch emails");
            return false;
        }
    }

    public async Task<List<string>> GetAvailableTemplatesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/email/templates", cancellationToken);
            response.EnsureSuccessStatusCode();

            var templates = await response.Content.ReadFromJsonAsync<List<TemplateInfo>>(cancellationToken: cancellationToken);
            return templates?.Select(t => t.Name).ToList() ?? new List<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get email templates");
            return new List<string>();
        }
    }

    public async Task<bool> VerifySmtpConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync("/api/email/verify", null, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify SMTP connection");
            return false;
        }
    }
}

public interface IEmailServiceClient
{
    Task<bool> SendEmailAsync(string to, string subject, string template, Dictionary<string, string> variables, CancellationToken cancellationToken = default);
    Task<bool> SendBatchEmailsAsync(IEnumerable<EmailServiceRequest> requests, CancellationToken cancellationToken = default);
    Task<List<string>> GetAvailableTemplatesAsync(CancellationToken cancellationToken = default);
    Task<bool> VerifySmtpConnectionAsync(CancellationToken cancellationToken = default);
}

public record EmailServiceRequest(
    string To,
    string Subject,
    string? Body = null,
    string? Template = null,
    Dictionary<string, string>? Variables = null,
    List<EmailAttachment>? Attachments = null);

public record EmailAttachment(
    string FileName,
    byte[] Data,
    string? ContentType = null);

public record EmailServiceResponse(
    bool Success,
    string? ErrorMessage = null,
    DateTime? SentAt = null);

public record TemplateInfo(string Name);
