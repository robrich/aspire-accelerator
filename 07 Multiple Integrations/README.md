# Multiple Integrations Demo

## Overview

This demo showcases how to build a sentiment analysis application that integrates multiple external services using Aspire:

- **Qdrant** - A vector database for storing and searching embeddings
- **Ollama** - A local LLM server for running AI models
  - **Multiple AI Models** - Both embedding and chat models running on Ollama

The application provides a sentiment analysis endpoint that:

1. Takes user input text
2. Generates embeddings using the `nomic-embed-text` model
3. Searches for similar sentiments in Qdrant using vector similarity
4. Uses the `llama3.2:1b` language model to summarize the emotional tone
5. Returns both the matching sentiment tags and a natural language summary

## Architecture

```
┌───────────────────────────────────────────────────────────────┐
│                   MultipleIntegrations.Api                    │
│                                                               │
│  GET /sentiment?value=<text>                                  │
│    ├─ Generate embedding with Ollama                          │
│    ├─ Search Qdrant for similar sentiments                    │
│    └─ Summarize with Ollama chat model                        │
└──────────────────────────┬────────────────────────────────────┘
                           │
        ┌──────────────────┼────────────────┐
        │                  │                │
    ┌───▼──────┐      ┌────▼─────┐    ┌─────▼──────┐
    │ Qdrant   │      │ Ollama   │    │Aspire Host │
    │ Vector   │      │ LLM      │    │            │
    │Database  │      │Server    │    │-Qdrant     │
    │          │      │          │    │-Ollama     │
    │Collection│      │Models:   │    │-Models     │
    │name:     │      │- nomic   │    └────────────┘
    │sentiments│      │  embed   │
    └──────────┘      │- llama   │
                      └──────────┘
```

## Prerequisites

- .NET 9 SDK or later
- Docker Desktop (for running Qdrant and Ollama containers)
- Visual Studio 2022, Visual Studio Code, or another .NET IDE
- Approximately 3-4 GB of disk space for the AI models

## Step-by-Step Instructions

### Step 1: Set Up User Secrets for API Key

The AppHost needs an API key to connect to the Qdrant dashboard from the browser.

1. Open a terminal in the `MultipleIntegrations.AppHost` directory
2. Initialize User Secrets if not already done:
   ```bash
   dotnet user-secrets init
   ```
3. Add the API key secret:
   ```bash
   dotnet user-secrets set "Parameters:apiKey" "qdrant-api-key-123"
   ```

### Step 2: Configure Qdrant Container in AppHost

In `MultipleIntegrations.AppHost/AppHost.cs`, add code to configure Qdrant:

```csharp
// Add the apiKey parameter from User Secrets so we
// can connect to the Qdrant dashboard from the browser.
var apiKey = builder.AddParameter("apiKey", secret: true);

// Construct the Qdrant db container with a persistent data volume
// and set its lifetime to Persistent so the container is
// not spun-down when the program ends. The container will
// download automatically if not already downloaded.
var qdrant = builder.AddQdrant("qdrant", apiKey)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);
```

**Key Points:**

- `AddParameter` retrieves the API key from User Secrets
- `.WithDataVolume()` creates persistent storage for the vector database
- `.WithLifetime(ContainerLifetime.Persistent)` keeps the container running between sessions

### Step 3: Configure Ollama Container in AppHost

Add the Ollama container with its models:

```csharp
// Construct the Ollama container with a 
// data volume, and persistent lifetime.
var ollama = builder
    .AddOllama("ollama")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

// Add the embedding model to Ollama
// The model will be downloaded automatically if not already present.
var embeddingModel = ollama
    .AddModel("nomic-embed-text");

// Add the language model to Ollama
// The model will be downloaded automatically if not already present.
var languageModel = ollama
    .AddModel("llama3.2:1b");
```

**Key Points:**

- Each `.AddModel()` call registers a model with Ollama
- Models are automatically downloaded on first run (this may take several minutes)
- Both models are stored persistently for reuse

### Step 4: Wire Up the API Project in AppHost

Connect the API to all external services:

```csharp
var api = builder
    .AddProject<Projects.MultipleIntegrations_Api>("api")
    .WithReference(qdrant)
    .WithReference(ollama)
    .WithReference(embeddingModel)
    .WithReference(languageModel)
    .WaitFor(qdrant)
    .WaitFor(embeddingModel)
    .WaitFor(languageModel);

builder.Build().Run();
```

**Key Points:**

- `.WithReference()` makes services discoverable to the API via environment variables
- `.WaitFor()` ensures services are ready before the API starts
- References include both container references and specific model references

### Step 5: Add Qdrant Client to API

In `MultipleIntegrations.Api/Program.cs`, add the Qdrant client to the service collection:

```csharp
// Add Qdrant Client
builder
    .AddQdrantClient("qdrant");
```

This uses the service name from the AppHost to automatically connect.

### Step 6: Add Ollama AI Services to API

Still in `Program.cs`, add the Ollama clients and generators:

```csharp
// Add Ollama Api Client
var ollamaApiClient = builder
    .AddOllamaApiClient("ollama");

// Add Ollama Chat Client
ollamaApiClient.AddChatClient();

// Add Ollama Embeddings Generator
ollamaApiClient.AddEmbeddingGenerator();
```

**Key Points:**

- The API client is the foundation for Ollama integration
- `.AddChatClient()` registers the chat model (llama3.2:1b)
- `.AddEmbeddingGenerator()` registers the embedding model (nomic-embed-text)

### Step 7: Seed the Qdrant Database

In `Program.cs`, after building the app, seed the sentiment data:

```csharp
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
```

**Key Points:**
- Uses dependency injection to get service instances
- Only seeds if the collection doesn't already exist
- Leverages extension methods for cleaner code

### Step 8: Create Extension Methods

Create helper classes in `MultipleIntegrations.Api/Extensions/`:

**OllamaClientExtensions.cs** - Configure models and add helper methods:

- `OllamaChatOptions` - Configure the chat model (temperature, top_p, penalties)
- `OllamaEmbeddingOptions` - Configure the embedding model
- `GetEmbeddingAsync()` - Generate embedding for a string
- `GetSentimentSummaryAsync()` - Get emotional summary from sentiment labels

**QdrantClientExtensions.cs** - Vector database operations:

- Collection configuration
- `SeedSentimentDbAsync()` - Load sentiment data and store embeddings
- Vector search helpers

**ScoredLabelExtensions.cs** - Sentiment label utilities:

- `MinOutlierScore()` - Filter out low-scoring labels

### Step 9: Inject Services into Controller

In `SentimentController.cs`, inject the required services:

```csharp
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
```

### Step 10: Implement the Sentiment Analysis Endpoint

Implement the GET endpoint:

```csharp
[HttpGet(Name = "GetSentiment")]
public async Task<GetSentimentResponse> Get(string value)
{
    // Step 1: Generate embedding for input text
    var embedding = await _embeddingsClient.GetEmbeddingAsync(value);
    
    // Step 2: Search Qdrant for similar sentiments
    var nearestSentiments = await _dbClient.SearchAsync(
        collectionName: QdrantClientExtensions.CollectionName,
        vector: embedding,
        limit: 200);

    // Step 3: Extract labels and filter outliers
    var labels = nearestSentiments.Select(s => 
        new ScoredLabel(s.Payload["label"].ToString(), s.Score));

    var minOutlierScore = labels.MinOutlierScore();
    var tags = labels.Where(l => l.Score >= minOutlierScore);

    // Step 4: Get AI-generated summary
    var summary = await _chatClient.GetSentimentSummaryAsync(tags.Select(l => l.Value));

    return new GetSentimentResponse(tags, summary);
}
```

**Key Points:**

- Embeddings enable semantic similarity search
- Vector search finds contextually related sentiments
- LLM summarizes the emotional character naturally
- Outlier filtering removes low-confidence matches

## Running the Demo

### Using Visual Studio or Rider:

1. Set `MultipleIntegrations.AppHost` as the startup project
2. Press F5 or click "Start Debugging"
3. Wait for Qdrant and Ollama to initialize (first run takes several minutes)
4. The Aspire dashboard will open automatically

### Using the Command Line:

```bash
cd MultipleIntegrations.AppHost
dotnet run
```

### Testing the Sentiment Endpoint

Once running, test the API using:

**Option 1: Using the .http file**
- Open `MultipleIntegrations.Api/Tests.http`
- Send the requests

**Option 2: Using curl**

```bash
curl "http://localhost:<port>/sentiment?value=This%20is%20amazing!"
```

**Option 3: Using the Swagger UI**

- Navigate to the API's OpenAPI endpoint in the Aspire dashboard
- Use the Swagger interface

## Aspire Dashboard

The Aspire dashboard provides visibility into:

- **Services** - Status of API, Qdrant, and Ollama
- **Resources** - Container logs and performance metrics
- **Environment Variables** - Service bindings and configuration
- **Trace Viewer** - Request tracing and performance analysis

Access it at the URL displayed when the AppHost starts.

## Understanding the Flow

1. **User Input** → User sends text to `/sentiment` endpoint
2. **Embedding Generation** → Ollama's `nomic-embed-text` converts text to vector
3. **Vector Search** → Qdrant searches for semantically similar sentiments
4. **Label Extraction** → Results are converted to scored sentiment labels
5. **Outlier Filtering** → Low-confidence labels are removed
6. **LLM Summarization** → Ollama's `llama3.2:1b` creates emotional summary
7. **Response** → Client receives labels and natural language summary

## Key Concepts

### Vector Embeddings

- Convert text into numerical vectors that capture semantic meaning
- Enable semantic similarity search (not just keyword matching)
- Essential for AI-powered applications

### Vector Database (Qdrant)

- Specialized for storing and searching embeddings
- Optimized for similarity queries in high-dimensional space
- Persistent storage ensures data survives between sessions

### Ollama

- Runs open-source LLMs locally
- Two models used: embedding model and chat model
- No cloud dependencies or API costs

### Aspire Orchestration

- Declares all service dependencies declaratively
- Manages container lifecycle automatically
- Provides environment variables for service discovery
- Orders startup with `.WaitFor()`

## Troubleshooting

### Models take a long time to download

- First run downloads and optimizes models (~2-4 GB total)
- Subsequent runs use cached models (much faster)
- Check Aspire dashboard for Ollama container logs

### Qdrant API key not recognized

- Verify User Secrets were set correctly
- Restart the AppHost after changing secrets
- Check the Qdrant container logs for connection issues

### API can't connect to services

- Ensure containers are running (check Aspire dashboard)
- Verify `.WaitFor()` calls are in place in AppHost
- Check environment variables in Aspire dashboard

### Out of memory errors

- Models require significant RAM (~4 GB+ depending on model size)
- Ensure Docker Desktop has sufficient memory allocated
- Monitor resource usage in Aspire dashboard

## Further Learning

- [Aspire Documentation](https://aspire.dev/)
- [Qdrant Documentation](https://qdrant.tech/documentation/)
- [Ollama Documentation](https://ollama.ai/)
- [Microsoft.Extensions.AI](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.ai)
- [Vector Embeddings Guide](https://en.wikipedia.org/wiki/Word_embedding)

## Summary

This demo illustrates the power of combining multiple AI/ML services with .NET Aspire:

- **Local execution** - No cloud dependencies
- **Simple integration** - Service bindings handle discovery
- **Production-ready** - Same patterns work in Docker, Kubernetes, Azure
- **Observable** - Aspire dashboard provides full visibility

By the end of this demo, you'll understand how to orchestrate complex microservices architectures with Aspire, integrate AI models locally, and build intelligent applications using vector search and LLMs.
