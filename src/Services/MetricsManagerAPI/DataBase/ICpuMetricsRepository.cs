using MetricsManagerAPI.Models;

namespace MetricsManagerAPI.DataBase;

public interface ICpuMetricsRepository
{
    Task<OperationResult<Guid>> CreateAsync(CreateCpuProcessorTimeTotalMetric metricDto);
    IList<GetCpuProcessorTimeTotalMetric> GetByTimePeriod(DateTimeOffset from, DateTimeOffset to);
}