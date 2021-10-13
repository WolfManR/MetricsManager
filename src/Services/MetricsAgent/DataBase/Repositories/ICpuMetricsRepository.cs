using MetricsAgent.Models;

namespace MetricsAgent.DataBase.Repositories;

public interface ICpuMetricsRepository
{
    void Create(CreateCpuProcessorTimeTotalMetric entity);
    IList<CpuProcessorTimeTotalMetric> GetByTimePeriod(DateTimeOffset from, DateTimeOffset to);
}