namespace MetricsManagerAPI.Models;

public record AgentInfo(Guid Id, string Uri)
{
    public bool IsEnabled { get; set; }

    public ICollection<CpuProcessorTimeTotalMetric> CpuProcessorTimeTotalMetrics { get; init; }
}