namespace SGS.OAD.DB
{
    public class ConsoleLogger : ILogger
    {
        public void LogInformation(string message)
        {
            LogToConsole(message, LogLevel.INF);
        }

        public void LogWarning(string message)
        {
            LogToConsole(message, LogLevel.WAR);
        }

        public void LogError(string message, Exception exception = null)
        {
            LogToConsole(message, LogLevel.ERR);

            if (exception != null)
                LogToConsole(exception.ToString(), LogLevel.ERR);
        }

        private static void LogToConsole(string message, LogLevel logLevel)
        {
            Console.WriteLine($"[{logLevel}] {DateTime.Now:yyyy/MM/dd HH:mm:ss}: {message}");
        }
    }
}
