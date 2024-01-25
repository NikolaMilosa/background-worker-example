using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var channel = Channel.CreateUnbounded<Guid>(new UnboundedChannelOptions
{
    SingleReader = true,
    SingleWriter = false,
});
builder.Services.AddSingleton(channel);
builder.Services.AddHostedService(x => new BackgroundGenerator<Guid>(x.GetRequiredService<ILogger<BackgroundGenerator<Guid>>>(), channel.Reader, 5));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

// app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapGet("/process", async ([FromServices] Channel<Guid> channel, uint num) =>
{
    for (int i = 0; i < num; i++)
    {
        await channel.Writer.WriteAsync(Guid.NewGuid());
    }

    return "Ok";
})
.WithName("Process")
.WithOpenApi();

app.Run("http://localhost:8000");

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
