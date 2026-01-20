using Qdrant.Client;
using Qdrant.Client.Grpc;
using Microsoft.Extensions.AI;

namespace MultipleIntegrations.Api.Extensions;

public static class QdrantClientExtensions
{
    public const string CollectionName = "sentiments";

    public static VectorParams VectorConfig => new VectorParams
    {
        Size = 768,
        Distance = Distance.Cosine
    };

    public async static Task CreateSentimentDbAsync(this QdrantClient client, string collectionName)
        => await client.CreateCollectionAsync(collectionName, VectorConfig);

    internal async static Task<Guid> AddSentimentAsync(this QdrantClient qdrantClient, IEmbeddingGenerator<string, Embedding<float>> embeddingClient, string collectionName, string sentimentLabel)
    {
        var sentiment = new Sentiment(sentimentLabel);
        var vector = await embeddingClient.GetEmbeddingAsync(sentiment.Value);

        var point = sentiment.AsPointStruct(vector);
        await qdrantClient.UpsertAsync(collectionName, new[] { point });

        return sentiment.Id;
    }

    public async static Task AddSentimentsAsync(this QdrantClient qdrantClient, IEmbeddingGenerator<string, Embedding<float>> embeddingsClient, string collectionName, string sentimentFilePath)
    {
        var sentimentLabels = File.ReadAllLines(sentimentFilePath);
        foreach (var sentimentLabel in sentimentLabels)
            await qdrantClient.AddSentimentAsync(embeddingsClient, collectionName, sentimentLabel);
    }

    public static async Task SeedSentimentDbAsync(this QdrantClient qdrantClient, IEmbeddingGenerator<string, Embedding<float>> embeddingsClient, string collectionName, string sentimentFilePath)
    {
        Console.WriteLine("Seeding Sentiment Database...");
        await qdrantClient.CreateSentimentDbAsync(collectionName);

        Console.WriteLine("Adding Sentiments to Database...");
        await qdrantClient.AddSentimentsAsync(embeddingsClient, collectionName, sentimentFilePath);

        Console.WriteLine("Sentiment Database Seeded.");
    }

}
