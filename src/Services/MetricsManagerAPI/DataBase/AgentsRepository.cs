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
        if (_context.Agents.Any(a => a.Uri == agent.Uri))
        {
            return new OperationResult<Guid>(Guid.Empty, true);
        }

        var entity = new AgentInfo(Guid.Empty, agent.Uri) { IsEnabled = agent.IsEnabled };

        await _context.Agents.AddAsync(entity);
        await _context.SaveChangesAsync();

        return new OperationResult<Guid>(entity.Id);
    }

    public async Task<OperationResult> EnableAgentAsync(Guid id)
    {
        var agent = await _context.Agents.SingleOrDefaultAsync(a => a.Id == id);

        if (agent is null) return new OperationResult(false);

        agent.IsEnabled = true;
        await _context.SaveChangesAsync();

        return new OperationResult();
    }

    public async Task<OperationResult> DisableAgentAsync(Guid id)
    {
        var agent = await _context.Agents.SingleOrDefaultAsync(a => a.Id == id);

        if (agent is null) return new OperationResult(false);

        agent.IsEnabled = false;
        await _context.SaveChangesAsync();

        return new OperationResult();
    }

    public async Task<OperationResult<IEnumerable<GetAgentEnableInfo>>> GetAgentsAsync()
    {
        var data = await _context.Agents.AsNoTracking().ToListAsync();
        if(!data.Any()) return new OperationResult<IEnumerable<GetAgentEnableInfo>>(Array.Empty<GetAgentEnableInfo>());
        return new OperationResult<IEnumerable<GetAgentEnableInfo>>(data.Select(x => new GetAgentEnableInfo(x.Uri, x.IsEnabled)));
    }

    public async Task<OperationResult<IEnumerable<GetAgent>>> GetEnabledAgentsAsync()
    {
        var data = await _context.Agents.AsNoTracking().Where(a => a.IsEnabled).ToListAsync();
        if (!data.Any()) return new OperationResult<IEnumerable<GetAgent>>(Array.Empty<GetAgent>());
        return new OperationResult<IEnumerable<GetAgent>>(data.Select(x => new GetAgent(x.Id, x.Uri)));
    }
}