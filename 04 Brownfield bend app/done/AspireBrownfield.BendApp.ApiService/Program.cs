using StackExchange.Redis;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// local debugging: "localhost:6379"
string? redisConnStr = builder.Configuration.GetConnectionString("cache"); // <-- no longer "Redis" because we named it "cache" in AppHost
ArgumentNullException.ThrowIfNullOrWhiteSpace(redisConnStr);

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    return ConnectionMultiplexer.Connect(redisConnStr);
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

string[] summaries = [ "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/", () => "API service is running. Navigate to /weatherforecast to see sample data.");

// Cached weather endpoint
app.MapGet("/api/weatherforecast", async (IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();
    const string cacheKey = "weatherforecast";
    var cached = await db.StringGetAsync(cacheKey);
    if (cached.HasValue)
    {
        var cachedForecast = JsonSerializer.Deserialize<WeatherForecast[]>(cached.ToString()!)!;
        return Results.Ok(cachedForecast);
    }

    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    var json = JsonSerializer.Serialize(forecast);
    // Set cache expiration (adjust TTL as needed)
    await db.StringSetAsync(cacheKey, json, TimeSpan.FromSeconds(3));

    return Results.Ok(forecast);
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
