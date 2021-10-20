using MetricsManagerAPI.Models;

namespace MetricsManagerAPI.Clients;

public interface ICpuMetricsClient
{
    Task<OperationResult<IReadOnlyCollection<AgentCpuMetricResponse>>> GetMetrics(string agentUri, DateTimeOffset fromTime, DateTimeOffset toTime);
}