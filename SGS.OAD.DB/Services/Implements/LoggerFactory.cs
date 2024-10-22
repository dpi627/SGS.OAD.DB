namespace SGS.OAD.DB
{
    /// <summary>
    /// 管理多種 Logger 實作
    /// </summary>
    public class LoggerFactory : ILogger
    {
        // 儲存所有 Logger
        private readonly List<ILogger> _loggers = new List<ILogger>();

        /// <summary>
        /// 可注入多種 Logger
        /// </summary>
        /// <param name="loggers">各種 Logger</param>
        public LoggerFactory(params ILogger[] loggers)
        {
            _loggers.AddRange(loggers);
        }

        public void LogInformation(string message)
        {
            foreach (var logger in _loggers)
            {
                logger.LogInformation(message);
            }
        }

        public void LogWarning(string message)
        {
            foreach (var logger in _loggers)
            {
                logger.LogWarning(message);
            }
        }

        public void LogError(string message, Exception exception = null)
        {
            foreach (var logger in _loggers)
            {
                logger.LogError(message, exception);
            }
        }
    }
}
