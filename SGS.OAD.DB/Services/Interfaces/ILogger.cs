namespace SGS.OAD.DB
{
    /// <summary>
    /// 簡易的 Logger 介面，方便外部實作注入
    /// </summary>
    public interface ILogger
    {
        void LogInformation(string message);
        void LogWarning(string message);
        void LogError(string message, Exception exception = null);
    }
}
