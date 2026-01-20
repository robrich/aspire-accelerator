namespace MultipleIntegrations.Api;

public class Program
{
    const string SentimentDataFilePath = @"Data/sentiments.txt";

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        // TODO: Add Qdrant Client

        // TODO: Add Ollama Api Client

        // TODO: Add Ollama Chat Client

        // TODO: Add Ollama Embeddings Generator

        var app = builder.Build();

        // TODO: Seed the database with sentiment data
        // Note: Ideally, this would be done using a creation script
        // in the AppHost. See demo 6, on SQL Server for an example.

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.MapControllers();
        app.Run();
    }
}
