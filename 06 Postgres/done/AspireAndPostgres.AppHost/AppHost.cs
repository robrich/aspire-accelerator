var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("username", secret: true);
var password = builder.AddParameter("password", secret: true);

var dbName = "weatherdb"; // because it already exists, we can fill it with content

var postgres = builder.AddPostgres("postgres", username, password)
    .WithEnvironment("POSTGRES_DB", dbName)
    .WithImageTag("alpine")
    //.WithDataVolume("postgres-data", isReadOnly: false); // volume is faster and saves in Docker between runs but does no save to local disk
    .WithDataBindMount(
        source: "../postgres-data",
        isReadOnly: false)
    .WithInitFiles("../postgres-init/init.sql")
    .WithPgWeb();

var weatherdb = postgres.AddDatabase(dbName);

var server = builder.AddProject<Projects.AspireAndPostgres_Server>("server")
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints()
    .WithReference(weatherdb)
    .WaitFor(postgres);

var webfrontend = builder.AddViteApp("webfrontend", "../frontend")
    .WithReference(server)
    .WaitFor(server)
    .WithEnvironment("NODE_OPTIONS", "--use-system-ca"); // make https work in dev

server.PublishWithContainerFiles(webfrontend, "wwwroot");

builder.Build().Run();
