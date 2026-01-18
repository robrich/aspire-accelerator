var builder = DistributedApplication.CreateBuilder(args);

var appServiceEnvironment = builder.AddAzureAppServiceEnvironment("appservice");

IResourceBuilder<IResourceWithConnectionString> cache = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureManagedRedis("cache")
    : builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.AspireToAppService_ApiService>("apiservice")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.AspireToAppService_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
