using MetricsManagerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MetricsManagerAPI.DataBase;

public class AgentsRepository : IAgentsRepository
{
    private readonly ManagerDbContext _context;

    public AgentsRepository(ManagerDbContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<Guid>> CreateAsync(CreateAgent agent)
    {
        var entity = new AgentInfo(Guid.Empty, agent.Uri) { IsEnabled = agent.IsEnabled };

        await _context.Agents.AddAsync(entity);
        await _context.SaveChangesAsync();

        return new OperationResult<Guid>(true, entity.Id);
    }

    public async Task<OperationResult> EnableAgentAsync(Guid id)
    {
        var agent = await _context.Agents.SingleOrDefaultAsync(a => a.Id == id);

        if (agent is null) return new OperationResult(false);

        agent.IsEnabled = true;
        await _context.SaveChangesAsync();

        return new OperationResult(true);
    }

    public async Task<OperationResult> DisableAgentAsync(Guid id)
    {
        var agent = await _context.Agents.SingleOrDefaultAsync(a => a.Id == id);

        if (agent is null) return new OperationResult(false);

        agent.IsEnabled = false;
        await _context.SaveChangesAsync();

        return new OperationResult(true);
    }

    public async Task<OperationResult<IEnumerable<GetAgent>>> GetAgentsAsync()
    {
        var data = await _context.Agents.ToListAsync();
        if(!data.Any()) return new OperationResult<IEnumerable<GetAgent>>(true, Array.Empty<GetAgent>());
        return new OperationResult<IEnumerable<GetAgent>>(true, data.Select(x => new GetAgent(x.Id, x.IsEnabled)));
    }
}