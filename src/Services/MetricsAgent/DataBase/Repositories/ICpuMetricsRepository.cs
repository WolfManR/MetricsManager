using MetricsAgent.Models;

namespace MetricsAgent.DataBase.Repositories;

public interface ICpuMetricsRepository
{
    Task<OperationResult<Guid>> CreateAsync(CreateCpuProcessorTimeTotalMetric entity);
    IList<CpuProcessorTimeTotalMetric> GetByTimePeriod(DateTimeOffset from, DateTimeOffset to);
}