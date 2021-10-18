namespace MetricsManagerAPI.Models;

public record CpuProcessorTimeTotalMetric(Guid Id, long RetrieveTime, int Value)
{
    public AgentInfo Agent { get; init; }
}

public record GetCpuProcessorTimeTotalMetric(Guid Id, long RetrieveTime, int Value);
public record CreateCpuProcessorTimeTotalMetric(AgentInfo Agent, long RetrieveTime, int Value);