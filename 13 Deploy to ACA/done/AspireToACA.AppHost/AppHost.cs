var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureContainerAppEnvironment("azurecontainerapps");

var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.AspireToACA_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.AspireToACA_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
