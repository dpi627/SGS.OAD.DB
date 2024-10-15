namespace SGS.OAD.DB.Models
{
    public class CacheItem
    {
        public DbInfo DbInfo { get; }
        public DateTime CreatedAt { get; }

        public CacheItem(DbInfo dbInfo)
        {
            DbInfo = dbInfo;
            CreatedAt = DateTime.UtcNow;
        }

        public bool IsExpired(TimeSpan expirationTime)
        {
            return DateTime.UtcNow - CreatedAt > expirationTime;
        }
    }

}
