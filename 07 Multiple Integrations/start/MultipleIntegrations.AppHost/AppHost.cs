var builder = DistributedApplication.CreateBuilder(args);

// TODO: Add the apiKey parameter from User Secrets so we
// can connect to the Qdrant dashboard from the browser.

// TODO: Construct the Qdrant db container with a persistent data volume
// and set its lifetime to Persistent so the container is
// not spun-down when the program ends.

// TODO: Construct the Ollama container with a 
// data volume, and persistent lifetime.

// TODO: Add the embedding model to Ollama
// The model will be downloaded automatically if not already present.

// TODO: Add the language model to Ollama
// The model will be downloaded automatically if not already present.

// TODO: setup the applicaton builder with:
// - a reference to the api project
// - references to the Qdrant container & Ollama container
// - references to the embedding model & language model
// - a wait for Qdrant, embedding model, and language model
//   to be ready before starting the api

builder.Build().Run();
