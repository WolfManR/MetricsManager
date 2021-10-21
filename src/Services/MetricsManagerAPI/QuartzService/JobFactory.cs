using Quartz.Spi;
using Quartz;

namespace MetricsManagerAPI.QuartzService;

public class JobFactory : IJobFactory
{
    private readonly IServiceProvider _provider;

    public JobFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        return _provider.GetRequiredService(bundle.JobDetail.JobType) as IJob;
    }

    public void ReturnJob(IJob job)
    {
    }
}