using GraphCalc.App.Services;
using GraphCalc.Domain.Interfaces;
using GraphCalc.Domain.Services;
using GraphCalc.Infrastructure.ExpressionEvaluation;
using GraphCalc.Infrastructure.GraphCalculation;
using GraphCalc.Infra.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<GraphAppService>();
builder.Services.AddScoped<UserAppService>();

builder.Services.AddScoped<GraphCalculationService>();

builder.Services.AddSingleton<IExpressionEvaluator, CodingSebExpressionEvaluator>();

builder.Services.AddSingleton<IGraphRepository, InMemoryGraphRepository>();
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();

builder.Services.AddScoped<IGraphCalculator, SimpleGraphCalculator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
