Adding a Database to Aspire: MS SQL Server
==========================================

This demo shows adding a database to the Aspire project.  In this example we'll add Microsoft SQL Server to the application running in a container.


Prerequisites
-------------

Ensure Docker Desktop or Podman is running.  You'll need this for the Redis container to start.


Steps
-----

You may choose to reference the tutorial at https://aspire.dev/integrations/databases/efcore/sql-server/ for more exhaustive steps.


### Instantiate the starter project

1. Into the `start` folder, use the Aspire Starter Template to create a project.

   If you're using the Aspire CLI, run this in the terminal:

   ```sh
   aspire new
   ```

2. Choose options that make you happy.

3. Run the project to ensure the site starts as expected and you get weather forecasts on the page.


### Add the database container to the AppHost project

1. In the Solution Explorer, right-click on the `AppHost` project, choose `Add Aspire Component` and in the NuGet dialog, add `Aspire.Hosting.SqlServer` to the project.

2. In `AppHost.cs`, add these lines right after the builder definition:

   ```csharp
   var password = builder.AddParameter("password", secret: true);

   var mssql = builder.AddSqlServer("mssql", password)
       .WithLifetime(ContainerLifetime.Persistent)
       //.WithDataVolume("mssql-data") // volumes are faster
       .WithDataBindMount(source: @"../mssql-data"); // bind mounts store data in a local folder

   string creationScript = File.ReadAllText("../mssql-init/init.sql");

   var weatherdb = mssql.AddDatabase("weatherdb")
       .WithCreationScript(creationScript);
   ```

   These instructions say:

   a. Define a password, storing it in the AppHost's User Secrets.
   b. (Not mentioned. The username is `sa`.)
   c. Define a SQL Server.
   d. Because it takes a long time to spin up SQL Server, don't shut the container down between runs.
   e. Store the data in the `mssql-data` folder.
   f. When the database container first starts, initialize it with `init.sql`, creating the database, tables, and seeding the data.

3. From the resources folder, move the mssql-init folder next to the AspireAndSqlServer.sln file:

   - `mssql-init/init.sql`

4. Right next to the mssql-init folder, create the `mssql-data` folder.

   This might get automatically created on first run, but creating it specifically is a good idea.

5. Add a reference from the database to the ApiServer project.

   In `AppHost.cs` change this:

   ```csharp
   var apiService = builder.AddProject<Projects.AspireAndSqlServer_ApiService>("apiservice")
       .WithHttpHealthCheck("/health");
   ```

   to

   ```csharp
   var apiService = builder.AddProject<Projects.AspireAndSqlServer_ApiService>("apiservice")
       .WithHttpHealthCheck("/health")
       .WithReference(weatherdb)
       .WaitFor(weatherdb);
   ```


### Add the database to the Server project

1. In the Solution Explorer, right-click on the `AspireAndSqlServer.ApiService` project, choose `Add Aspire Component` and in the NuGet dialog, add `Aspire.Microsoft.EntityFrameworkCore.SqlServer` to the project.

2. In Program.cs, add this line towards the top:

    ```csharp
    builder.AddSqlServerDbContext<WeatherDbContext>("weatherdb");
    ```

    Notice that we're using the same database name we used in AppHost.cs.

3. Copy the Data folder from the resources folder into the project:

   - `Data/WeatherForecast.cs`
   - `Data/WeatherDbContext.cs`

4. Towards the bottom of `Program.cs`, change the Minimal API method to call Entity Framework:

   ```csharp
   api.MapGet("/weatherforecast", () =>
   {
       var forecast = Enumerable.Range(1, 5).Select(index =>
           new WeatherForecast
           (
               DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
               Random.Shared.Next(-20, 55),
               summaries[Random.Shared.Next(summaries.Length)]
           ))
           .ToArray();
       return forecast;
   })
   .WithName("GetWeatherForecast");
   ```

   to

   ```csharp
   api.MapGet("weatherforecast", (WeatherDbContext db) =>
   {
       return db.WeatherForecasts.ToList();
   })
   .WithName("GetWeatherForecast");
   ```


### Debug the Project

1. Set the AppHost project as the startup project.

2. Start debugging the solution.  If running from the CLI:

   ```sh
   aspire run
   ```

3. As the dashboard loads, it'll note that you don't have the password set.

4. Click `Set Parameters`

   - Set the password to anything that makes you happy with sufficient complexity
   - Choose to save these in user secrets

5. Restart the project, and it should now launch successfully.

6. Open the website to ensure it works as expected.
