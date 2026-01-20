using MultipleIntegrations.Api.Extensions;

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

        // Add Qdrant Client
        builder
            .AddQdrantClient("qdrant");

        // Add Ollama Api Client
        var ollamaApiClient = builder
            .AddOllamaApiClient("ollama");

        // Add Ollama Chat Client
        ollamaApiClient.AddChatClient();

        // Add Ollama Embeddings Generator
        ollamaApiClient.AddEmbeddingGenerator();

        var app = builder.Build();

        // Seed the database with sentiment data
        // Note: Ideally, this would be done using a creation script
        // in the AppHost. See demo 6, on SQL Server for an example.
        using (var scope = app.Services.CreateScope())
        {
            var dbClient = scope.ServiceProvider.GetRequiredService<Qdrant.Client.QdrantClient>();
            var embeddingsClient = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.AI.IEmbeddingGenerator<string, Microsoft.Extensions.AI.Embedding<float>>>();

            if (!await dbClient.CollectionExistsAsync(QdrantClientExtensions.CollectionName))
                await dbClient.SeedSentimentDbAsync(embeddingsClient, QdrantClientExtensions.CollectionName, SentimentDataFilePath);
        }

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
