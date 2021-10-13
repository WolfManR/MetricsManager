namespace MetricsAgent.Models
{
    public record CpuProcessorTimeTotalMetric(Guid Id, long RetrieveTime, int Value);
    public record CreateCpuProcessorTimeTotalMetric(long RetrieveTime, int Value);
}
