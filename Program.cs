using GraphCalc.Infrastructure.Persistence;
using GraphCalc.Infrastructure.Repositories;
using GraphCalc.Infrastructure.ExpressionEvaluation;
using GraphCalc.Domain.Interfaces;
using GraphCalc.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Infrastructure
builder.Services.AddSingleton<IExpressionEvaluator, CodingSebExpressionEvaluator>();
builder.Services.AddSingleton<IGraphCalculator, GraphCalc.Infrastructure.GraphCalculation.NumericalGraphCalculator>();

// Repositories
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<IGraphSetRepository, InMemoryGraphSetRepository>();

// Domain Services
builder.Services.AddScoped<IGraphService, GraphService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();