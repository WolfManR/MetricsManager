namespace MetricsManagerAPI.Clients;

public record AgentCpuMetricResponse(DateTimeOffset RetrieveTime, int Value);