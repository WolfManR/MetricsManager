using MetricsManagerAPI.Models;

namespace MetricsManagerAPI.DataBase;

public interface ICpuMetricsRepository
{
    Task<OperationResult<Guid>> CreateAsync(CreateCpuProcessorTimeTotalMetric metricDto);
    Task<OperationResult<DateTimeOffset>> GetAgentLastMetricDate(Guid agentId);
    IList<GetCpuProcessorTimeTotalMetric> GetByTimePeriod(DateTimeOffset from, DateTimeOffset to);
    Task AddMetricsChunK(IEnumerable<CreateCpuProcessorTimeTotalMetric> chunk);
    IList<GetCpuProcessorTimeTotalMetric> GetByTimePeriod(Guid agentId, DateTimeOffset from, DateTimeOffset to);
}