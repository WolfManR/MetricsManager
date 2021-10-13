using MetricsAgent.DataBase.Repositories;
using Quartz;
using System.Diagnostics;

namespace MetricsAgent.QuartzServices.Jobs;

public class CpuProcessorTimeTotalJob : IJob
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
            _repository.CreateAsync(new(time, cpuMetric));
        }
        catch (OverflowException e)
        {
            _logger.LogError("Cant create cpu metric, metric value overflow limit of integer", e);
        }
        catch (Exception e)
        {
            _logger.LogError(101, "Cant save cpu metric", e);
        }
        return Task.CompletedTask;
    }
}