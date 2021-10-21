using MetricsManagerAPI.Clients;
using MetricsManagerAPI.DataBase;
using MetricsManagerAPI.Models;

using Quartz;

namespace MetricsManagerAPI.QuartzService.MetricJobs;

[DisallowConcurrentExecution]
public class CpuMetricJob : IJob
{
    private readonly ICpuMetricsRepository _metricsRepository;
    private readonly ICpuMetricsClient _client;
    private readonly IAgentsRepository _agentsRepository;

    public CpuMetricJob(ICpuMetricsRepository metricsRepository, ICpuMetricsClient client, IAgentsRepository agentsRepository)
    {
        _metricsRepository = metricsRepository;
        _client = client;
        _agentsRepository = agentsRepository;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var result = await _agentsRepository.GetEnabledAgentsAsync();
        if (!result.IsSuccess) return;
        var metrics = await Task.WhenAll(result.Result.Select(a => GetAgentMetrics(a.Id, a.Uri)));
        // Add metrics
        foreach (var chunk in metrics.SelectMany(enumerable => enumerable).Chunk(8))
        {
           await _metricsRepository.AddMetricsChunK(chunk);
        }
    }

    private async Task<IEnumerable<CreateCpuProcessorTimeTotalMetric>> GetAgentMetrics(Guid id, string uri)
    {
        return await GetMetricsByTimePeriod(id, uri, await GetLastMetricDate(id), DateTimeOffset.UtcNow).ConfigureAwait(false);
    }

    private async Task<DateTimeOffset> GetLastMetricDate(Guid agentId)
    {
        var (lastDate, isSuccess) = await _metricsRepository.GetAgentLastMetricDate(agentId);
        return isSuccess ? lastDate : DateTimeOffset.UtcNow;
    }

    private async Task<IEnumerable<CreateCpuProcessorTimeTotalMetric>> GetMetricsByTimePeriod(Guid agentId, string agentUri, DateTimeOffset from, DateTimeOffset to)
    {
        var (result, isSuccess) = await _client.GetMetrics(agentUri, @from, to).ConfigureAwait(false);
        if (!isSuccess) return Array.Empty<CreateCpuProcessorTimeTotalMetric>();
        var metrics = result.Select(r => new CreateCpuProcessorTimeTotalMetric(agentId, r.RetrieveTime.ToUnixTimeMilliseconds(), r.Value)).ToArray();
        return metrics;
    }
}