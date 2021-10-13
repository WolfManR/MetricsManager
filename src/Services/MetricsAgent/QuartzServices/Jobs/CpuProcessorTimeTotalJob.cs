using MetricsAgent.DataBase.Repositories;
using Quartz;
using System.Data.SQLite;
using System.Diagnostics;

namespace MetricsAgent.QuartzServices.Jobs;

public class CpuProcessorTimeTotalJob
{
    private readonly ICpuMetricsRepository _repository;
    private readonly ILogger<CpuProcessorTimeTotalJob> _logger;
    private readonly PerformanceCounter _counter;

    public CpuProcessorTimeTotalJob(ICpuMetricsRepository repository, ILogger<CpuProcessorTimeTotalJob> logger)
    {
        _repository = repository;
        _logger = logger;
        _counter = new("Processor", "% Processor Time", "_Total");
    }

    public Task Execute(IJobExecutionContext context)
    {
        try
        {
            var cpuMetric = Convert.ToInt32(_counter.NextValue());
            var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            _repository.Create(new(time, cpuMetric));
        }
        catch (OverflowException e)
        {
            _logger.LogError("Cant create cpu metric, metric value overflow limit of integer", e);
        }
        catch (SQLiteException e) when (e.Message.Contains("no such table"))
        {
            _logger.LogDebug("Table for cpu metrics still not exist");
        }
        catch (Exception e)
        {
            _logger.LogError(101, "Cant save cpu metric", e);
        }
        return Task.CompletedTask;
    }
}