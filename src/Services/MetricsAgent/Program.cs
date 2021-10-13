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

preBuildServices.AddDbContext<AgentDbContext>(options => options.UseSqlite("Data Source=db.sqlite3"));
preBuildServices.AddSingleton<ICpuMetricsRepository, CpuMetricsRepository>();

preBuildServices.AddSingleton<IJobFactory, JobFactory>();
preBuildServices.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
preBuildServices.AddHostedService<QuartzHostedService>();

preBuildServices.AddJob<CpuProcessorTimeTotalJob>("0/5 * * * * ?");

preBuildServices.AddControllers();
preBuildServices.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "MetricsAgent", Version = "v1" });
});

var app = builder.Build();

var postBuildServices = app.Services;

var context = postBuildServices.GetRequiredService<AgentDbContext>();
context.Database.Migrate();
context.Dispose();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MetricsAgent v1"));
}

app.UseAuthorization();

app.MapControllers();

app.Run();
