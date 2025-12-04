using GraphCalc.Infrastructure.Facade;
using GraphCalc.Infrastructure.Persistence;
using GraphCalc.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<GraphCalculationFacade>();
builder.Services.AddSingleton<IGraphRepository, InMemoryGraphRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();