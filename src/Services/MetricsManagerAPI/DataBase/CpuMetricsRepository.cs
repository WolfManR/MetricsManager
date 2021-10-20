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
        CpuProcessorTimeTotalMetric entity = new(Guid.Empty, metricDto.RetrieveTime, metricDto.Value);
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

    private static (long, long) GetCorrectTimeValuesRange(DateTimeOffset @from, DateTimeOffset to)
    {
        if (@from > to) return (to.ToUnixTimeSeconds(), @from.ToUnixTimeSeconds());
        return (@from.ToUnixTimeSeconds(), to.ToUnixTimeSeconds());
    }
}