using MailKit.Net.Smtp;
using MimeKit;
using Polly;

namespace EmailService.Api;

public class EmailSenderService : IEmailSenderService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailSenderService> _logger;
    private readonly ITemplateService _templateService;
    private readonly IRateLimitingService _rateLimitingService;
    private int _emailsSentToday = 0;
    private int _emailsSentThisHour = 0;
    private DateTime _hourResetTime = DateTime.UtcNow;

    public EmailSenderService(
        IConfiguration configuration,
        ILogger<EmailSenderService> logger,
        ITemplateService templateService,
        IRateLimitingService rateLimitingService)
    {
        _configuration = configuration;
        _logger = logger;
        _templateService = templateService;
        _rateLimitingService = rateLimitingService;
    }

    public async Task<bool> SendEmailAsync(EmailRequest request, CancellationToken cancellationToken = default)
    {
        // Проверка rate limiting
        if (!_rateLimitingService.AllowSend())
        {
            _logger.LogWarning("Rate limit exceeded. Email not sent to {To}", request.To);
            throw new RateLimitExceededException("Email rate limit exceeded");
        }

        try
        {
            var message = await CreateMessageAsync(request, cancellationToken);

            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(exception, "Retry {RetryCount} sending email to {To}", retryCount, request.To);
                    });

            await retryPolicy.ExecuteAsync(async () =>
            {
                using var client = new SmtpClient();
                await client.ConnectAsync(
                    _configuration["Smtp:Host"],
                    int.Parse(_configuration["Smtp:Port"] ?? "587"),
                    bool.Parse(_configuration["Smtp:EnableSsl"] ?? "true"),
                    cancellationToken);

                // Аутентификация если есть credentials
                var username = _configuration["Smtp:Username"];
                var password = _configuration["Smtp:Password"];
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    await client.AuthenticateAsync(username, password, cancellationToken);
                }

                await client.SendAsync(message, cancellationToken);
                await client.DisconnectAsync(true, cancellationToken);
            });

            _rateLimitingService.RecordSent();
            _logger.LogInformation("Email sent successfully to {To} with subject {Subject}", request.To, request.Subject);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", request.To);
            throw;
        }
    }

    public async Task<bool> SendBatchEmailsAsync(IEnumerable<EmailRequest> requests, CancellationToken cancellationToken = default)
    {
        var results = new List<bool>();
        foreach (var request in requests)
        {
            try
            {
                var result = await SendEmailAsync(request, cancellationToken);
                results.Add(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send batch email to {To}", request.To);
                results.Add(false);
            }
        }
        return results.All(r => r);
    }

    private async Task<MimeMessage> CreateMessageAsync(EmailRequest request, CancellationToken cancellationToken)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            _configuration["Smtp:FromName"] ?? "MetaFlow",
            _configuration["Smtp:FromEmail"] ?? "noreply@metaflow.com"));
        message.To.Add(MailboxAddress.Parse(request.To));
        message.Subject = request.Subject;

        // Если есть шаблон, используем его
        if (!string.IsNullOrEmpty(request.Template))
        {
            var body = await _templateService.RenderTemplateAsync(request.Template, request.Variables, cancellationToken);
            message.Body = new TextPart("html")
            {
                Text = body
            };
        }
        else
        {
            // Plain text или HTML из request
            message.Body = new TextPart("html")
            {
                Text = request.Body ?? string.Empty
            };
        }

        // Добавление вложений
        if (request.Attachments?.Any() == true)
        {
            var multipart = new Multipart("mixed");
            multipart.Add(message.Body);

            foreach (var attachment in request.Attachments)
            {
                var part = new MimePart(attachment.ContentType ?? "application/octet-stream")
                {
                    Content = new MimeContent(new MemoryStream(attachment.Data)),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = attachment.FileName
                };
                multipart.Add(part);
            }

            message.Body = multipart;
        }

        return message;
    }
}

public interface IEmailSenderService
{
    Task<bool> SendEmailAsync(EmailRequest request, CancellationToken cancellationToken = default);
    Task<bool> SendBatchEmailsAsync(IEnumerable<EmailRequest> requests, CancellationToken cancellationToken = default);
}

public record EmailRequest(
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
