using System.Collections.Immutable;
using System.Text.Json;
using MetricsManagerAPI.Models;

namespace MetricsManagerAPI.Clients;

public class CpuMetricsClient : ICpuMetricsClient
{
    private readonly HttpClient _client;

    public CpuMetricsClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<OperationResult<IReadOnlyCollection<AgentCpuMetricResponse>>> GetMetrics(string agentUri, DateTimeOffset fromTime, DateTimeOffset toTime)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, agentUri);
        var response = await _client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<IEnumerable<AgentCpuMetricResponse>>(content, new JsonSerializerOptions(JsonSerializerDefaults.Web));

            return new OperationResult<IReadOnlyCollection<AgentCpuMetricResponse>>(result.ToImmutableArray());
        }

        return new OperationResult<IReadOnlyCollection<AgentCpuMetricResponse>>(Array.Empty<AgentCpuMetricResponse>(), false);
    }
}