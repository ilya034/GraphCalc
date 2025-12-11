using GraphCalc.Infrastructure.Persistence;
using GraphCalc.Infrastructure.Repositories;
using GraphCalc.Infrastructure.ExpressionEvaluation;
using GraphCalc.Domain.Interfaces;
using GraphCalc.Domain.Services;
using GraphCalc.Presentation.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IExpressionEvaluator, CodingSebExpressionEvaluator>();
builder.Services.AddSingleton<IGraphRepository, InMemoryGraphRepository>();
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<InMemoryPublishedGraphRepository>();
builder.Services.AddSingleton<InMemoryGraphSetRepository>();

// Регистрация доменных сервисов
builder.Services.AddScoped<IGraphCalculationService, GraphCalculationService>();
builder.Services.AddScoped<IUserService, UserService>();

// Регистрация сервисов презентационного слоя
builder.Services.AddScoped<GraphCalc.Presentation.Services.IGraphDisplayService, GraphCalc.Presentation.Services.GraphDisplayService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();