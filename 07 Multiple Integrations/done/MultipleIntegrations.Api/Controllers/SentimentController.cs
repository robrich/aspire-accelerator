using Microsoft.AspNetCore.Mvc;
using Qdrant.Client;
using Microsoft.Extensions.AI;
using MultipleIntegrations.Api.Extensions;

namespace MultipleIntegrations.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SentimentController : ControllerBase
{
    private readonly QdrantClient _dbClient;
    private readonly IChatClient _chatClient;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingsClient;

    public SentimentController(
        QdrantClient dbClient,
        IChatClient chatClient,
        IEmbeddingGenerator<string, Embedding<float>> embeddingsClient)
    {
        _dbClient = dbClient;
        _chatClient = chatClient;
        _embeddingsClient = embeddingsClient;
    }

    [HttpGet(Name = "GetSentiment")]
    public async Task<GetSentimentResponse> Get(string value)
    {
        var embedding = await _embeddingsClient.GetEmbeddingAsync(value);
        var nearestSentiments = await _dbClient.SearchAsync(
            collectionName: QdrantClientExtensions.CollectionName,
            vector: embedding,
            limit:200);

        var labels = nearestSentiments.Select(s => 
            new ScoredLabel(s.Payload["label"].ToString(), s.Score));

        var minOutlierScore = labels.MinOutlierScore();
        var tags = labels.Where(l => l.Score >= minOutlierScore);

        var summary = await _chatClient.GetSentimentSummaryAsync(tags.Select(l => l.Value));

        return new GetSentimentResponse(tags, summary);
    }
}
