using MetricsManagerAPI.Clients;
using MetricsManagerAPI.DataBase;
using MetricsManagerAPI.QuartzService;
using MetricsManagerAPI.QuartzService.MetricJobs;
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


app.MapGet("/", () => "Hello Metrics!")
.WithName("Hello Metrics");

app.Run();
