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

app.MapPost("/api/agents/register",
    async ([FromBody] string uri, [FromBody] bool isEnabled, IAgentsRepository agentsRepository, ILogger<WebApplication> logger) =>
    {
        var result = await agentsRepository.CreateAsync(new CreateAgent(uri, isEnabled));
        if (!result.IsSuccess)
        {
            return Results.BadRequest("Agent may be registered");
        }
        return Results.Ok(result.Result);
    });

app.MapPut("/api/agents/enable/{agentId:Guid}",
    async ([FromRoute] Guid agentId, IAgentsRepository agentsRepository, ILogger<WebApplication> logger) =>
    {
        if (agentId == Guid.Empty) return Results.BadRequest("Wrong Id");
        var result = await agentsRepository.EnableAgentAsync(agentId);
        if (!result.IsSuccess) return Results.BadRequest("Fail to enable agent");
        return Results.Ok();
    });

app.MapPut("/api/agents/disable/{agentId:Guid}",
    async ([FromRoute] Guid agentId, IAgentsRepository agentsRepository, ILogger<WebApplication> logger) =>
    {
        if (agentId == Guid.Empty) return Results.BadRequest("Wrong Id");
        var result = await agentsRepository.DisableAgentAsync(agentId);
        if (!result.IsSuccess) return Results.BadRequest("Fail to disable agent");
        return Results.Ok();
    });

app.MapGet("/api/agents",
    async (IAgentsRepository agentsRepository, ILogger<WebApplication> logger) =>
    {
        var result = await agentsRepository.GetAgentsAsync();
        if (!result.IsSuccess) return Results.BadRequest("Fail to get agents");
        return Results.Ok(result.Result);
    });


app.Run();
