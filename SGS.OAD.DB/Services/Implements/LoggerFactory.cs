namespace SGS.OAD.DB
{
    public class LoggerFactory : ILogger
    {
        private readonly bool _isEnable; // = ConfigHelper.GetValue<bool>("ENABLE_LOG");
        private readonly List<ILogger> _loggers = new List<ILogger>();

        public LoggerFactory(bool isEnable = false, params ILogger[] loggers)
        {
            _isEnable = isEnable;
            if (!_isEnable) return;
            _loggers.AddRange(loggers);
        }

        public void LogInformation(string message)
        {
            if (!_isEnable) return;
            foreach (var logger in _loggers)
            {
                logger.LogInformation(message);
            }
        }

        public void LogWarning(string message)
        {
            if (!_isEnable) return;
            foreach (var logger in _loggers)
            {
                logger.LogWarning(message);
            }
        }

        public void LogError(string message, Exception exception = null)
        {
            if (!_isEnable) return;
            foreach (var logger in _loggers)
            {
                logger.LogError(message, exception);
            }
        }
    }
}
