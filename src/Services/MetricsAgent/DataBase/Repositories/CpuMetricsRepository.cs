using MetricsAgent.Models;

using static Dapper.SqlMapper;

namespace MetricsAgent.DataBase.Repositories;
public class CpuMetricsRepository : ICpuMetricsRepository
{
    private readonly IConnectionBuilder _connectionBuilder;
    private string TableName { get; } = SD.CpuProcessorTimeTotalTableName;

    protected CpuMetricsRepository(IConnectionBuilder connectionBuilder)
    {
        _connectionBuilder = connectionBuilder;
    }

    public void Create(CreateCpuProcessorTimeTotalMetric entity)
    {
        using var connection = _connectionBuilder.CreateSQLiteConnection();
        var result = connection.Execute(
            $"INSERT INTO {TableName}(Value,RetrieveTime) VALUES (@value,@time);",
            new
            {
                value = entity.Value,
                time = entity.RetrieveTime
            }
        );

        if (result <= 0)
        {
            throw new InvalidOperationException("Failure to add entity to database")
            {
                Data =
                {
                    ["value"] = entity.Value,
                    ["time"] = entity.RetrieveTime
                }
            };
        }
    }

    public IList<CpuProcessorTimeTotalMetric> GetByTimePeriod(DateTimeOffset from, DateTimeOffset to)
    {
        var fromSeconds = from.ToUnixTimeSeconds();
        var toSeconds = to.ToUnixTimeSeconds();

        using var connection = _connectionBuilder.CreateSQLiteConnection();
        string command;
        object commandParameters;
        if (fromSeconds == toSeconds)
        {
            command = $"SELECT * FROM {TableName} WHERE (RetrieveTime = @from);";
            commandParameters = new { from = fromSeconds };
        }
        else
        {
            command = $"SELECT * FROM {TableName} WHERE (RetrieveTime > @from) and (RetrieveTime < @to);";
            commandParameters = fromSeconds > toSeconds
                ? new { from = toSeconds, to = fromSeconds }
                : new { from = fromSeconds, to = toSeconds };
        }

        return connection.Query<CpuProcessorTimeTotalMetric>(command, commandParameters).ToList();
    }
}