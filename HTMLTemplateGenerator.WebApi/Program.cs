using HTMLTemlateGenerator.Infrastructure;
using HTMLTemplateGenerator.Application;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.ApplicationDependencies();
builder.Services.InfrastructureDependencies();

var app = builder.Build();

app.MapControllers();

app.Run();
