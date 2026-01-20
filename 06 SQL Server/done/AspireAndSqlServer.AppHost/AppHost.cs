var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("password", secret: true);

var mssql = builder.AddSqlServer("mssql", password)
    .WithLifetime(ContainerLifetime.Persistent)
    //.WithDataVolume("mssql-data") // volumes are faster
    .WithDataBindMount(source: @"../mssql-data"); // bind mounts store data in a local folder

string creationScript = File.ReadAllText("../mssql-init/init.sql");

var weatherdb = mssql.AddDatabase("weatherdb")
    .WithCreationScript(creationScript);

var apiService = builder.AddProject<Projects.AspireAndSqlServer_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(weatherdb)
    .WaitFor(weatherdb);

builder.AddProject<Projects.AspireAndSqlServer_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
