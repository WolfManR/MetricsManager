using MetricsManagerAPI.Models;

namespace MetricsManagerAPI.DataBase
{
    public interface IAgentsRepository
    {
        Task<OperationResult<Guid>> CreateAsync(CreateAgent agent);
        Task<OperationResult> DisableAgentAsync(Guid id);
        Task<OperationResult> EnableAgentAsync(Guid id);
        Task<OperationResult<IEnumerable<GetAgentEnableInfo>>> GetAgentsAsync();
        Task<OperationResult<IEnumerable<GetAgent>>> GetEnabledAgentsAsync();
    }
}