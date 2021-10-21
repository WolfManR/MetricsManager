using MetricsManagerAPI.Clients;
using MetricsManagerAPI.DataBase;
using MetricsManagerAPI.Models;
using MetricsManagerAPI.QuartzService;
using MetricsManagerAPI.QuartzService.MetricJobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quartz.Impl;

using Quartz;
using Quartz.Spi;
using System;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var preBuildServices = builder.Services;

preBuildServices.AddDbContext<ManagerDbContext>((provider, options) =>
{
    var configuration = provider.GetService<IConfiguration>();
    options.UseNpgsql(configuration.GetConnectionString("Default"));
});

preBuildServices.AddHttpClient<ICpuMetricsClient, CpuMetricsClient>();

preBuildServices.AddSingleton<IJobFactory, JobFactory>();
preBuildServices.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
preBuildServices.AddHostedService<QuartzHostedService>();

preBuildServices.AddJob<CpuMetricJob>("0/5 * * * * ?");

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
preBuildServices.AddEndpointsApiExplorer();
preBuildServices.AddSwaggerGen();

var app = builder.Build();

var context = app.Services.GetRequiredService<ManagerDbContext>();
await context.Database.MigrateAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Agents
app.MapPost("/api/agents/register",
    async ([FromBody] string uri, [FromBody] bool isEnabled, IAgentsRepository agentsRepository, ILogger<WebApplication> logger) =>
    {
        var result = await agentsRepository.CreateAsync(new CreateAgent(uri, isEnabled));
        if (!result.IsSuccess)
        {
            logger.LogWarning("Fail to register agent by uri: {uri}", uri);
            return Results.BadRequest("Agent may be registered");
        }
        logger.LogInformation("Successfully register agent with uri: {uri}", uri);
        return Results.Ok(result.Result);
    });

app.MapPut("/api/agents/enable/{agentId:Guid}",
    async ([FromRoute] Guid agentId, IAgentsRepository agentsRepository, ILogger<WebApplication> logger) =>
    {
        if (agentId == Guid.Empty)
        {
            logger.LogWarning("Get request to enable agent with empty id");
            return Results.BadRequest("Wrong Id");
        }

        var result = await agentsRepository.EnableAgentAsync(agentId);
        if (!result.IsSuccess)
        {
            logger.LogWarning("Fail to enable agent on Id: {id}", agentId);
            return Results.BadRequest("Fail to enable agent");
        }
        logger.LogInformation("Successfully enable agent on Id: {id}", agentId);
        return Results.Ok();
    });

app.MapPut("/api/agents/disable/{agentId:Guid}",
    async ([FromRoute] Guid agentId, IAgentsRepository agentsRepository, ILogger<WebApplication> logger) =>
    {
        if (agentId == Guid.Empty)
        {
            logger.LogWarning("Get request to disable agent with empty id");
            return Results.BadRequest("Wrong Id");
        }
        var result = await agentsRepository.DisableAgentAsync(agentId);
        if (!result.IsSuccess)
        {
            logger.LogWarning("Fail to disable agent on Id: {id}", agentId);
            return Results.BadRequest("Fail to disable agent");
        }
        logger.LogInformation("Successfully disable agent on Id: {id}", agentId);
        return Results.Ok();
    });

app.MapGet("/api/agents",
    async (IAgentsRepository agentsRepository, ILogger<WebApplication> logger) =>
    {
        var result = await agentsRepository.GetAgentsAsync();
        if (!result.IsSuccess)
        {
            logger.LogWarning("Fail to send list of agents");
            return Results.BadRequest("Fail to get agents");
        }
        logger.LogInformation("Successfully send list of agents");
        return Results.Ok(result.Result);
    });

// Cpu Metrics
app.MapGet("/api/cpumetrics/processortime/total/cluster/from/{fromTime:DateTimeOffset}/to/{toTime:DateTimeOffset}",
    ([FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime, ICpuMetricsRepository cpuMetricsRepository, ILogger<WebApplication> logger) =>
    {
        var result = cpuMetricsRepository.GetByTimePeriod(fromTime, toTime);
        logger.LogInformation("Successfully send list of cpu metrics of processor time total");
        return Results.Ok(result);
    });

// Cpu Metrics
app.MapGet("/api/cpumetrics/processortime/total/agent/{agentId:Guid}/from/{fromTime:DateTimeOffset}/to/{toTime:DateTimeOffset}",
    ([FromRoute] Guid agentId, [FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime, ICpuMetricsRepository cpuMetricsRepository, ILogger<WebApplication> logger) =>
    {
        var result = cpuMetricsRepository.GetByTimePeriod(agentId, fromTime, toTime);
        logger.LogInformation("Successfully send list of cpu metrics of processor time total for agent: {agentId}", agentId);
        return Results.Ok(result);
    });


app.Run();
