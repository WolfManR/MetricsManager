namespace MetricsManagerAPI.Models;

public record AgentInfo(Guid Id, string Uri)
{
    public bool IsEnabled { get; set; }
}

public record CreateAgent(string Uri, bool IsEnabled);
public record GetAgent(Guid Id, string Uri);
public record GetAgentEnableInfo(string Uri, bool IsEnabled);