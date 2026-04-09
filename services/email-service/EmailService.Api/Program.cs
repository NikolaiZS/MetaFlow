using EmailService.Api;
using FluentValidation;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Настройка Serilog
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.Debug();
});

builder.Services.AddEndpointsApiExplorer();

// Регистрация сервисов
builder.Services.AddSingleton<IEmailSenderService, EmailSenderService>();
builder.Services.AddSingleton<ITemplateService, TemplateService>();
builder.Services.AddSingleton<IRateLimitingService, RateLimitingService>();

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<EmailRequestValidator>();

// Health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Middleware
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Health checks endpoint
app.MapHealthChecks("/health");

// ENDPOINTS

// Проверить подключение к SMTP
app.MapPost("/api/email/verify", async (
    IEmailSenderService emailService,
    ILogger<IEmailSenderService> logger,
    CancellationToken ct) =>
{
    try
    {
        // TODO: Реализовать проверку подключения
        return Results.Ok(new { Success = true, Message = "SMTP connection successful" });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "SMTP connection failed");
        return Results.BadRequest(new { Success = false, Message = ex.Message });
    }
})
.WithName("VerifySmtpConnection")
.WithOpenApi();

// Отправка одного письма
app.MapPost("/api/email/send", async (
    EmailRequest request,
    IEmailSenderService emailService,
    ILogger<IEmailSenderService> logger,
    CancellationToken ct) =>
{
    try
    {
        var success = await emailService.SendEmailAsync(request, ct);
        return Results.Ok(new EmailSendResponse(Success: success, SentAt: DateTime.UtcNow));
    }
    catch (RateLimitExceededException ex)
    {
        logger.LogWarning(ex.Message);
        return Results.StatusCode(429);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to send email to {To}", request.To);
        return Results.BadRequest(new EmailSendResponse(Success: false, ErrorMessage: ex.Message));
    }
})
.WithName("SendEmail")
.WithOpenApi();

// Массовая отправка писем
app.MapPost("/api/email/send-batch", async (
    IEnumerable<EmailRequest> requests,
    IEmailSenderService emailService,
    ILogger<IEmailSenderService> logger,
    CancellationToken ct) =>
{
    try
    {
        var success = await emailService.SendBatchEmailsAsync(requests, ct);
        return Results.Ok(new EmailSendResponse(Success: success, SentAt: DateTime.UtcNow));
    }
    catch (RateLimitExceededException ex)
    {
        logger.LogWarning(ex.Message);
        return Results.StatusCode(429);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to send batch emails");
        return Results.BadRequest(new EmailSendResponse(Success: false, ErrorMessage: ex.Message));
    }
})
.WithName("SendBatchEmails")
.WithOpenApi();

// Получить список доступных шаблонов
app.MapGet("/api/email/templates", async (
    ITemplateService templateService,
    CancellationToken ct) =>
{
    var templates = await templateService.GetAvailableTemplatesAsync();
    return Results.Ok(templates.Select(t => new { Name = t }));
})
.WithName("GetTemplates")
.WithOpenApi();

app.Run();
