using Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Shared.Services;
public class CoreLogService : ICoreLogService
{
    private readonly ILogger _logger;

    public CoreLogService(ILogger<CoreLogService> logger)
    {
        _logger = logger;
    }

    public void LogError(string message)
    {
        _logger.LogError("{Message}", message);
    }

    public void LogError(string template, params object[] args)
    {
        _logger.LogError(template, args);
    }

    public void LogInformation(string message)
    {
        _logger.LogInformation("{Message}", message);
    }

    public void LogInformation(string template, params object[] args)
    {
        _logger.LogInformation(template, args);
    }

    public void LogWarning(string message)
    {
        _logger.LogWarning("{Message}", message);
    }

    public void LogWarning(string template, params object[] args)
    {
        _logger.LogWarning(template, args);
    }
}
