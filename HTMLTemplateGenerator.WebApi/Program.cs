using HTMLTemlateGenerator.Infrastructure;
using HTMLTemplateGenerator.Application;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();

    });
});

builder.Services.ApplicationDependencies();
builder.Services.InfrastructureDependencies();

var app = builder.Build();

app.UseCors("AllowFrontend");

app.MapControllers();

app.Run();
