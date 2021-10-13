using System.Data.SQLite;

namespace MetricsAgent.DataBase;

public class Connection : IConnectionBuilder, IConnectionConfigurationStage
{
    private string _connectionString;

    private Connection()
    {

    }

    public static IConnectionConfigurationStage Create()
    {
        return new Connection();
    }

    public SQLiteConnection CreateSQLiteConnection() => new SQLiteConnection(_connectionString);
    public IConnectionBuilder WithConnectionString(string connectionString)
    {
        _connectionString = connectionString;
        return this;
    }
}

public interface IConnectionConfigurationStage
{
    public IConnectionBuilder WithConnectionString(string connectionString);
}

public interface IConnectionBuilder
{
    public SQLiteConnection CreateSQLiteConnection();
}