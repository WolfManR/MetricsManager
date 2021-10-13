using System.Data.SQLite;

namespace MetricsAgent.DataBase;

public class Connection : ICreateConnectionStage, IConnectionConfigurationStage
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
    public ICreateConnectionStage WithConnectionString(string connectionString)
    {
        _connectionString = connectionString;
        return this;
    }
}

public interface IConnectionConfigurationStage
{
    public ICreateConnectionStage WithConnectionString(string connectionString);
}

public interface ICreateConnectionStage
{
    public SQLiteConnection CreateSQLiteConnection();
}