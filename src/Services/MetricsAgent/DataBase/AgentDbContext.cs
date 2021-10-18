﻿using MetricsAgent.Models;

using Microsoft.EntityFrameworkCore;

namespace MetricsAgent.DataBase;

public class AgentDbContext : DbContext
{
    public AgentDbContext(DbContextOptions<AgentDbContext> options) : base(options)
    {

    }

    public DbSet<CpuProcessorTimeTotalMetric> CpuProcessorTimeTotalMetrics { get; init; }
}