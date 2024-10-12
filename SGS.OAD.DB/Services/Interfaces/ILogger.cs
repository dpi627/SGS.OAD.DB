using SGS.OAD.DB.Enums;

namespace SGS.OAD.DB.Services.Interfaces
{
    public interface ILogger
    {
        void Log(LogLevel level, string message);
        void LogError(string message, Exception ex = null);
        void LogInformation(string message);
        void LogWarning(string message);
    }

}
