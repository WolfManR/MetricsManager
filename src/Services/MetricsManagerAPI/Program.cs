using MetricsManagerAPI.DataBase;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var preBuildServices = builder.Services;

preBuildServices.AddDbContext<ManagerDbContext>((provider, options) =>
{
    var configuration = provider.GetService<IConfiguration>();
    options.UseNpgsql(configuration.GetConnectionString("Default"));
});

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
