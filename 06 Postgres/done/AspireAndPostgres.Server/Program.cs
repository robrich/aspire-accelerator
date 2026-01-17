using AspireAndPostgres.Server.Data;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Add Postgres
builder.AddNpgsqlDbContext<WeatherDbContext>("weatherdb"); // <-- matches database name in AppHost

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Swagger/OpenAPI (Swashbuckle)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AspireAndPostgres API v1"));
    app.MapOpenApi();
}

var api = app.MapGroup("/api");
api.MapGet("weatherforecast", (WeatherDbContext db) =>
{
    return db.WeatherForecasts.ToList();
})
.WithName("GetWeatherForecast");

// So loading the home page doesn't get a 404
app.MapGet("/", () =>
{
    return new { Message = "Welcome to AspireAndPostgres.Server!" };
});

app.MapDefaultEndpoints();

app.UseFileServer();

app.Run();
