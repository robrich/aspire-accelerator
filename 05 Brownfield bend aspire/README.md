Brownfield app: Bend Aspire
===========================

This demo shows adding Aspire to an existing app, and modifying the Aspire AppHost to match the application's conventions.

Sometimes we'd like to add Aspire, but we want to change the application as little as possible.  Perhaps the DevOps pipeline or the deployed instance assume conventions that are difficult to change.  In this case, we can bend Aspire to match the Application's existing needs.


Prerequisites
-------------

Ensure Docker Desktop or Podman is running.  You'll need this for the Redis container to start.


Steps
-----

1. In Visual Studio, open `start/AspireBrownfield.BendAspire.sln`.

2. In the Solution Explorer, right-click on the `AspireBrownfield.BendAspire.ApiService` project, choose `Add`, and choose `.NET Aspire Orchestrator Support`.

3. Accept the defaults and click OK.

4. Note that this process modifies `AspireBrownfield.BendAspire.ApiService/Program.cs` with references to the new `ServiceDefaults` project.

5. In the Solution Explorer, right-click on the `AspireBrownfield.Web` project, and again choose `Add` then `.NET Aspire Orchestrator Support`.

6. Confirm that you'll add to the existing Aspire projects.

7. Note that this process changed `AspireBrownfield.BendAspire.Web/Program.cs` like it did for the API project.

8. Note that both projects are now listed in `AspireBrownfield.BendAspire.AppHost/AppHost.cs` but are not connected together yet.  Note also the Redis cache is missing.

9. In the `AppHost` project, add a NuGet reference to `Aspire.Hosting.Redis`.

10. In the `AppHost` project, modify `AppHost.cs` to include Redis, connect Redis to both projects, and connect the API to the website:

    ```csharp
    var builder = DistributedApplication.CreateBuilder(args);

    var cache = builder.AddRedis("cache")
        .WithImageTag("alpine")
        .WithBindMount("../redis-data", "/data");

    var apiService = builder.AddProject<Projects.AspireBrownfield_BendAspire_ApiService>("apiservice")
        .WithReference(cache)
        .WaitFor(cache)
        .WithHttpHealthCheck("/health")
        .WithExternalHttpEndpoints();

    var web = builder.AddProject<Projects.AspireBrownfield_BendAspire_Web>("web")
        .WithExternalHttpEndpoints()
        .WithHttpHealthCheck("/health")
        .WithReference(cache)
        .WaitFor(cache)
        .WithReference(apiService)
        .WaitFor(apiService);

    builder.Build().Run();
    ```

11. Open `AspireBrownfield.BendApp.Web/Program.cs` and notice the naming conventions:

    - The Redis connection string is named `Redis`
    - The reference to the API is named `AppSettings:WeatherApiUrl`.

    Optional: open `appsettings.json` and look at the conventions there too.

12. The web apps don't use the conventions Aspire presents here.  Let's modify Aspire's AppHost to use the app's existing conventions.

    Open `AppHost` project's `AppHost.cs` and modify:

    ```csharp
    var cache = builder.AddRedis("cache")
    ```

    to

    ```csharp
    var cache = builder.AddRedis("Redis")
    ```

    By naming it to match the app's convention, we'll pass in the connection string the way the app expects.

13. Still in `AppHost.cs`, modify the `web` setup to include this line:

    ```csharp
    .WithEnvironment("AppSettings__WeatherApiUrl", apiService.GetEndpoint("HTTPS"));
    ```

    We've explicitly set an environment variable to `AppSettings:WeatherApiUrl` to the apiservice's https endpoint.  Yes, it will still pass the same data the other way, but now it'll pass it this way too.

14. Optional: Delete `docker-compose.yaml` as we now start the Redis container through Aspire instead.

15. Set the `AppHost` project as the startup project (if it isn't already).

16. Start debugging the application and ensure it still works as expected.
