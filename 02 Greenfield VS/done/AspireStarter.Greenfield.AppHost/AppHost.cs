var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var server = builder.AddProject<Projects.AspireStarter_Greenfield_Server>("server")
    .WithReference(cache)
    .WaitFor(cache)
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints();

var webfrontend = builder.AddViteApp("webfrontend", "../frontend")
    .WithReference(server)
    .WaitFor(server)
    .WithEnvironment("NODE_OPTIONS", "--use-system-ca"); // make https work in dev

server.PublishWithContainerFiles(webfrontend, "wwwroot");

builder.Build().Run();
