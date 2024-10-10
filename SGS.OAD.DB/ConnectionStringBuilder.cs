namespace SGS.OAD.DB;

public class ConnectionStringBuilder
{
    private string _server = string.Empty;
    private string _database = string.Empty;
    private string _connectionStringTemplate = @"server={0}, db={1}";

    public static ConnectionStringBuilder Empty() => new();

    public ConnectionStringBuilder SetSever(string serverName)
    {
        _server = serverName;
        return this;
    }

    public ConnectionStringBuilder SetDatabase(string databaseName)
    {
        _database = databaseName;
        return this;
    }

    public ConnectionStringBuilder SetTemplate(string connectionStringTemplate)
    {
        _connectionStringTemplate = connectionStringTemplate;
        return this;
    }

    public string Build()
    {
        return string.Format(_connectionStringTemplate, _server, _database);
    }
}
