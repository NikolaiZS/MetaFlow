namespace EmailService.Api;

public class TemplateService : ITemplateService
{
    private readonly ILogger<TemplateService> _logger;
    private readonly string _templatesPath;

    public TemplateService(ILogger<TemplateService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _templatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");
    }

    public async Task<string> RenderTemplateAsync(
        string templateName,
        Dictionary<string, string>? variables = null,
        CancellationToken cancellationToken = default)
    {
        var templatePath = Path.Combine(_templatesPath, $"{templateName}.html");

        if (!File.Exists(templatePath))
        {
            _logger.LogWarning("Template {TemplateName} not found at {TemplatePath}", templateName, templatePath);
            throw new TemplateNotFoundException($"Template '{templateName}' not found");
        }

        var content = await File.ReadAllTextAsync(templatePath, cancellationToken);

        // Замена переменных в шаблоне
        if (variables != null)
        {
            foreach (var (key, value) in variables)
            {
                content = content.Replace($"{{{{{key}}}}}", value);
            }
        }

        return content;
    }

    public async Task<List<string>> GetAvailableTemplatesAsync()
    {
        if (!Directory.Exists(_templatesPath))
        {
            return new List<string>();
        }

        return Directory.GetFiles(_templatesPath, "*.html")
            .Select(Path.GetFileNameWithoutExtension)
            .Where(name => !string.IsNullOrEmpty(name))
            .ToList()!;
    }
}

public interface ITemplateService
{
    Task<string> RenderTemplateAsync(string templateName, Dictionary<string, string>? variables = null, CancellationToken cancellationToken = default);
    Task<List<string>> GetAvailableTemplatesAsync();
}

public class TemplateNotFoundException : Exception
{
    public TemplateNotFoundException(string message) : base(message) { }
}
