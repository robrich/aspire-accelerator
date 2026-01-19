var builder = DistributedApplication.CreateBuilder(args);

// Configure model container
// Note: This will download the container if needed
var ollama = builder.AddOllama("ollama")
    .WithDataVolume();

// Configure model inside container
// Note: This will download the model if needed
var llamaModel = ollama.AddModel("llama3.2:1b");

// Connect console app
var wow = builder
    .AddProject<Projects.ModelConsole>("modelConsole")
    .WaitFor(llamaModel)
    .WithReference(ollama);

builder.Build().Run();
