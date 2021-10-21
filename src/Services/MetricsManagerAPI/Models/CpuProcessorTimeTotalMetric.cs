namespace MetricsManagerAPI.Models;

public record CpuProcessorTimeTotalMetric(long RetrieveTime, int Value, Guid AgentId)
{
    public Guid Id { get; set; }
}

public record GetCpuProcessorTimeTotalMetric(Guid Id, long RetrieveTime, int Value);
public record CreateCpuProcessorTimeTotalMetric(Guid AgentId, long RetrieveTime, int Value);