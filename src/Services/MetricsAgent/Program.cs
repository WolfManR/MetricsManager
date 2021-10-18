using MetricsAgent.DataBase;
using MetricsAgent.DataBase.Repositories;
using MetricsAgent.QuartzServices;
using MetricsAgent.QuartzServices.Jobs;

using Microsoft.EntityFrameworkCore;

using Quartz;
using Quartz.Impl;
using Quartz.Spi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var preBuildServices = builder.Services;

builder.Services.AddDbContext<AgentDbContext>((provider, options) =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    options.UseNpgsql(configuration.GetConnectionString("Default"));
}, ServiceLifetime.Singleton, ServiceLifetime.Singleton);

preBuildServices.AddSingleton<ICpuMetricsRepository, CpuMetricsRepository>();

preBuildServices.AddSingleton<IJobFactory, JobFactory>();
preBuildServices.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
preBuildServices.AddHostedService<QuartzHostedService>();

preBuildServices.AddJob<CpuProcessorTimeTotalJob>("0/5 * * * * ?");

preBuildServices.AddEndpointsApiExplorer();
preBuildServices.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "MetricsAgent", Version = "v1" });
});

var app = builder.Build();

var postBuildServices = app.Services;

var context = postBuildServices.GetRequiredService<AgentDbContext>();
await context.Database.MigrateAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MetricsAgent v1"));
}

app.MapGet("/api/cpu/processortime/total/", (ICpuMetricsRepository cpuMetricsRepository) =>
{
    var result = cpuMetricsRepository.GetByTimePeriod(DateTimeOffset.Now, DateTimeOffset.Now.AddDays(1));
    return Results.Ok(result);
});

app.Run();
