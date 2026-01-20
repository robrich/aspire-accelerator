Brownfield app: Bend the app
============================

This demo shows adding Aspire to an existing app, and modifying the app to match Aspire's conventions.

If we have control over the application, sometimes it's easier to bend the application to match Aspire's built-in conventions.  We'll use this approach here.


Prerequisites
-------------

Ensure Docker Desktop or Podman is running.  You'll need this for the Redis container to start.


Steps
-----

1. In Visual Studio, open `start/AspireBrownfield.BendApp.sln`.

2. In the Solution Explorer, right-click on the `AspireBrownfield.BendApp.ApiService` project, choose `Add`, and choose `.NET Aspire Orchestrator Support`.

3. Accept the defaults and click OK.

4. Note that this process modifies `AspireBrownfield.BendApp.ApiService/Program.cs` with references to the new `ServiceDefaults` project.

5. In the Solution Explorer, right-click on the `AspireBrownfield.Web` project, and again choose `Add` then `.NET Aspire Orchestrator Support`.

6. Confirm that you'll add to the existing Aspire projects.

7. Note that this process changed `AspireBrownfield.BendApp.Web/Program.cs` like it did for the API project.

8. Note that both projects are now listed in `AspireBrownfield.BendApp.AppHost/AppHost.cs` but are not connected together yet.  Note also the Redis cache is missing.

9. In the `AppHost` project, add a NuGet reference to `Aspire.Hosting.Redis`.

10. In the `AppHost` project, modify `AppHost.cs` to include Redis, connect Redis to both projects, and connect the API to the website:

    ```csharp
    var builder = DistributedApplication.CreateBuilder(args);

    var cache = builder.AddRedis("cache")
        .WithImageTag("alpine")
        .WithBindMount("../redis-data", "/data");

    var apiService = builder.AddProject<Projects.AspireBrownfield_BendApp_ApiService>("apiservice")
        .WithReference(cache)
        .WaitFor(cache)
        .WithHttpHealthCheck("/health")
        .WithExternalHttpEndpoints();

    var web = builder.AddProject<Projects.AspireBrownfield_BendApp_Web>("web")
        .WithExternalHttpEndpoints()
        .WithHttpHealthCheck("/health")
        .WithReference(cache)
        .WaitFor(cache)
        .WithReference(apiService)
        .WaitFor(apiService);

    builder.Build().Run();
    ```

11. The web apps don't use the conventions Aspire presents here.  Let's modify the apps to use Aspire's conventions.

12. Open `AspireBrownfield.BendApp.ApiService/Program.cs` and modify:

    ```csharp
    string? redisConnStr = builder.Configuration.GetConnectionString("Redis");
    ```

    to

    ```csharp
    string? redisConnStr = builder.Configuration.GetConnectionString("cache");
    ```

    In the Aspire AppHost project, we called this `cache`, so we use that same name here.

13. Optional: Update ApiService's `appsettings.json` and `appsettings.Development.json` to rename `Redis` to `cache` the changes we've made above.

14. Open `AspireBrownfield.BendApp.Web/Program.cs` and modify:

    ```csharp
    string? redisConnStr = builder.Configuration.GetConnectionString("Redis");
    ```

    to

    ```csharp
    string? redisConnStr = builder.Configuration.GetConnectionString("cache");
    ```

    Like we did in the ApiService project, we need to rename this to match Aspire's convention.

15. Also in `AspireBrownfield.BendApp.Web/Program.cs`, modify:

    ```csharp
    string? apiUrl = builder.Configuration.GetValue<string>("AppSettings:WeatherApiUrl");
    ```

    to

    ```csharp
    string? apiUrl = builder.Configuration.GetValue<string>("apiservice_https");
    ```

    Aspire passes in the environment variable to match the name of the service we gave it in `AppHost.cs`, so we'll modify the configuration name to match.

16. Optional: Update Web's `appsettings.json` and `appsettings.Development.json` to match the changes we made above.

17. Optional: Delete `docker-compose.yaml` as we now start the Redis container through Aspire instead.

18. Set the `AppHost` project as the startup project (if it isn't already).

19. Start debugging the application and ensure it still works as expected.
