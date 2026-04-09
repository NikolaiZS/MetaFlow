namespace EmailService.Api;

public class RateLimitingService : IRateLimitingService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RateLimitingService> _logger;
    private int _emailsSentThisHour = 0;
    private int _emailsSentToday = 0;
    private DateTime _hourResetTime = DateTime.UtcNow;
    private DateTime _dayResetTime = DateTime.UtcNow.Date;
    private readonly object _lock = new();

    public RateLimitingService(IConfiguration configuration, ILogger<RateLimitingService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public bool AllowSend()
    {
        lock (_lock)
        {
            ResetCountersIfNeeded();

            var maxPerHour = int.Parse(_configuration["RateLimiting:MaxEmailsPerHour"] ?? "100");
            var maxPerDay = int.Parse(_configuration["RateLimiting:MaxEmailsPerDay"] ?? "1000");

            if (_emailsSentThisHour >= maxPerHour)
            {
                _logger.LogWarning("Hourly rate limit exceeded ({Count}/{Max})", _emailsSentThisHour, maxPerHour);
                return false;
            }

            if (_emailsSentToday >= maxPerDay)
            {
                _logger.LogWarning("Daily rate limit exceeded ({Count}/{Max})", _emailsSentToday, maxPerDay);
                return false;
            }

            return true;
        }
    }

    public void RecordSent()
    {
        lock (_lock)
        {
            ResetCountersIfNeeded();
            _emailsSentThisHour++;
            _emailsSentToday++;
        }
    }

    private void ResetCountersIfNeeded()
    {
        var now = DateTime.UtcNow;

        if (now >= _hourResetTime.AddHours(1))
        {
            _emailsSentThisHour = 0;
            _hourResetTime = now;
        }

        if (now >= _dayResetTime.AddDays(1))
        {
            _emailsSentToday = 0;
            _dayResetTime = now.Date;
        }
    }
}

public interface IRateLimitingService
{
    bool AllowSend();
    void RecordSent();
}
