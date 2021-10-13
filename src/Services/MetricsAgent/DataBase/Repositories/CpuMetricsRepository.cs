using MetricsAgent.Models;
using Microsoft.EntityFrameworkCore;

namespace MetricsAgent.DataBase.Repositories;
public class CpuMetricsRepository : ICpuMetricsRepository
{
    private readonly AgentDbContext _context;

    protected CpuMetricsRepository(AgentDbContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<Guid>> CreateAsync(CreateCpuProcessorTimeTotalMetric metricDto)
    {
        CpuProcessorTimeTotalMetric entity = new(Guid.Empty, metricDto.RetrieveTime, metricDto.Value);
        await _context.CpuProcessorTimeTotalMetrics.AddAsync(entity);
        await _context.SaveChangesAsync();
        return new OperationResult<Guid>( Result: entity.Id );
    }

    public IList<CpuProcessorTimeTotalMetric> GetByTimePeriod(DateTimeOffset from, DateTimeOffset to)
    {
        var fromSeconds = from.ToUnixTimeSeconds();
        var toSeconds = to.ToUnixTimeSeconds();

        
        if (fromSeconds == toSeconds)
        {
            return _context.CpuProcessorTimeTotalMetrics
                .AsNoTracking()
                .Where(m => m.RetrieveTime == fromSeconds)
                .ToList();
        }

        return _context.CpuProcessorTimeTotalMetrics
            .AsNoTracking()
            .Where(m => m.RetrieveTime > fromSeconds && m.RetrieveTime < toSeconds)
            .ToList();
    }
}