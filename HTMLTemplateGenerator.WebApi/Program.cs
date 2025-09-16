using HTMLTemlateGenerator.Infrastructure;
using HTMLTemplateGenerator.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.ApplicationDependencies();
builder.Services.InfrastructureDependencies();

var app = builder.Build();

app.MapControllers();

app.Run();
