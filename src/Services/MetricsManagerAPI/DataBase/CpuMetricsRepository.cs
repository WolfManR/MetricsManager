using MetricsManagerAPI.Models;

using Microsoft.EntityFrameworkCore;

namespace MetricsManagerAPI.DataBase;

public class CpuMetricsRepository : ICpuMetricsRepository
{
    private readonly ManagerDbContext _context;

    public CpuMetricsRepository(ManagerDbContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<Guid>> CreateAsync(CreateCpuProcessorTimeTotalMetric metricDto)
    {
        CpuProcessorTimeTotalMetric entity = new(metricDto.RetrieveTime, metricDto.Value, metricDto.AgentId);
        await _context.CpuProcessorTimeTotalMetrics.AddAsync(entity);
        await _context.SaveChangesAsync();
        return new OperationResult<Guid>(Result: entity.Id);
    }

    public IList<GetCpuProcessorTimeTotalMetric> GetByTimePeriod(DateTimeOffset from, DateTimeOffset to)
    {
        var (fromSeconds, toSeconds) = GetCorrectTimeValuesRange(from, to);

        if (fromSeconds == toSeconds)
        {
            return _context.CpuProcessorTimeTotalMetrics
                .AsNoTracking()
                .Where(m => m.RetrieveTime == fromSeconds)
                .Select(m => new GetCpuProcessorTimeTotalMetric(m.Id, m.RetrieveTime, m.Value))
                .ToList();
        }

        return _context.CpuProcessorTimeTotalMetrics
            .AsNoTracking()
            .Where(m => m.RetrieveTime > fromSeconds && m.RetrieveTime < toSeconds)
            .Select(m => new GetCpuProcessorTimeTotalMetric(m.Id, m.RetrieveTime, m.Value))
            .ToList();
    }

    public async Task AddMetricsChunK(IEnumerable<CreateCpuProcessorTimeTotalMetric> chunk)
    {
        var entries = chunk.Select(metric => new CpuProcessorTimeTotalMetric(metric.RetrieveTime, metric.Value, metric.AgentId));
        await _context.CpuProcessorTimeTotalMetrics.AddRangeAsync(entries);
        await _context.SaveChangesAsync();
    }

    private static (long, long) GetCorrectTimeValuesRange(DateTimeOffset @from, DateTimeOffset to)
    {
        if (@from > to) return (to.ToUnixTimeSeconds(), @from.ToUnixTimeSeconds());
        return (@from.ToUnixTimeSeconds(), to.ToUnixTimeSeconds());
    }

    public async Task<OperationResult<DateTimeOffset>> GetAgentLastMetricDate(Guid agentId)
    {
        var maxTime = await _context.CpuProcessorTimeTotalMetrics
            .AsNoTracking()
            .Where(m => m.AgentId == agentId)
            .MaxAsync(m => m.RetrieveTime);

        if (maxTime >= 0)
        {
            return new OperationResult<DateTimeOffset>(Result: DateTimeOffset.FromUnixTimeSeconds(maxTime));
        }

        return new OperationResult<DateTimeOffset>(DateTimeOffset.UtcNow, false);
    }
}