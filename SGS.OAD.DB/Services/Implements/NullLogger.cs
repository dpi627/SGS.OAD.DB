
namespace SGS.OAD.DB
{
    public class NullLogger : ILogger
    {
        public void LogError(string message, Exception exception = null)
        {
        }

        public void LogInformation(string message)
        {
        }

        public void LogWarning(string message)
        {
        }
    }
}
