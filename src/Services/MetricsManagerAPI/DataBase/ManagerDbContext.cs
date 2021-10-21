using MetricsManagerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MetricsManagerAPI.DataBase;

public class ManagerDbContext : DbContext
{
    public ManagerDbContext(DbContextOptions<ManagerDbContext> options) : base(options)
    {

    }

    public DbSet<AgentInfo> Agents { get; init; }
    public DbSet<CpuProcessorTimeTotalMetric> CpuProcessorTimeTotalMetrics { get; init; }
}