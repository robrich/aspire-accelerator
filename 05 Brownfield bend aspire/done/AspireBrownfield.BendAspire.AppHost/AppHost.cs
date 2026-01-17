var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("Redis") // <-- not "cache" because the app calls it "Redis"
    .WithImageTag("alpine")
    .WithBindMount("../redis-data", "/data");

var apiService = builder.AddProject<Projects.AspireBrownfield_BendAspire_ApiService>("apiservice")
    .WithReference(cache, "Redis") // <-- or rename "cache" to "Redis"
    .WaitFor(cache)
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints();

var web = builder.AddProject<Projects.AspireBrownfield_BendAspire_Web>("web")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithEnvironment("AppSettings__WeatherApiUrl", apiService.GetEndpoint("HTTPS")); // <-- set specific env var for the app

builder.Build().Run();
