using GraphCalc.App.Services;
using GraphCalc.Domain.Interfaces;
using GraphCalc.Domain.Services;
using GraphCalc.Infra.ExpressionEvaluation;
using GraphCalc.Infra.GraphCalculation;
using GraphCalc.Infra.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Настройка CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("ViteDevPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Vite dev server
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

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
app.UseCors("ViteDevPolicy");

app.MapControllers();
app.Run();
