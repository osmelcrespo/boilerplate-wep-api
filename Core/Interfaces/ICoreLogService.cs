namespace Core.Interfaces;
public interface ICoreLogService
{
    void LogInformation(string message);
    void LogInformation(string template, params object[] args);
    void LogError(string message);
    void LogError(string template, params object[] args);
    void LogWarning(string message);
    void LogWarning(string template, params object[] args);
}
