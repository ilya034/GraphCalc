using GraphCalc.Domain.Interfaces;
using GraphCalc.Infrastructure.Facade;
using GraphCalc.Infrastructure.Persistence;
using GraphCalc.Infrastructure.Repositories;
using System.Text.Json;

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

builder.Services.AddScoped<GraphCalculationFacade>();
builder.Services.AddSingleton<IGraphRepository, InMemoryGraphRepository>();
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<InMemoryPublishedGraphRepository>();
builder.Services.AddSingleton<InMemoryGraphSetRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("ViteDevPolicy");

app.MapControllers();

app.Run();
