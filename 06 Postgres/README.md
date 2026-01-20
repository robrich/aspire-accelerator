Adding a Database to Aspire: Postgres
=====================================

This demo shows adding a database to the Aspire project.  In this example we'll add PostgreSQL to the application running in a container.


Prerequisites
-------------

Ensure Docker Desktop or Podman is running.  You'll need this for the Redis container to start.


Steps
-----

You may choose to reference the tutorial at https://aspire.dev/integrations/databases/efcore/postgresql/ for more exhaustive steps.


### Instantiate the starter project

1. Into the `start` folder, use the Aspire Starter Template to create a project.

   If you're using the Aspire CLI, run this in the terminal:

   ```sh
   aspire new
   ```

2. Choose options that make you happy.

3. Run the project to ensure the site starts as expected and you get weather forecasts on the page.


### Add the database container to the AppHost project

1. In the Solution Explorer, right-click on the `AppHost` project, choose `Add Aspire Component` and in the NuGet dialog, add `Aspire.Hosting.PostgreSQL` to the project.

2. In `AppHost.cs`, add these lines right after the builder definition:

   ```csharp
   var username = builder.AddParameter("username", secret: true);
   var password = builder.AddParameter("password", secret: true);

   var dbName = "weatherdb"; // because it already exists, we can fill it with content

   var postgres = builder.AddPostgres("postgres", username, password)
       .WithImageTag("alpine")
       .WithEnvironment("POSTGRES_DB", dbName)
       .WithInitFiles("../postgres-init/init.sql")
       .WithDataBindMount(
           source: "../postgres-data",
           isReadOnly: false)
       .WithPgWeb();

   var weatherdb = postgres.AddDatabase(dbName);
   ```

   These instructions say:

   a. Define a username and password, storing them in the AppHost's User Secrets.
   b. Define a Postgres server.
   c. Use the `postgres:alpine` container.
   d. Set Postgres to automatically create the `weatherdb` on initialization.
   e. Then run `init.sql` to create the table and seed it with data.
   f. Save the data to the `postgres-data` folder.
   g. Initialize the `sosedoff/pgweb:latest` container as an admin tool and connect it to the Postgres container.

   **Note**: We're not using the `.WithCreationScript(creationScript)` syntax because this runs in the Postgres database and not in the context of the new database we want to create.

3. From the resources folder, move the postgres-init folder next to the AspireAndPostgres.sln file:

   - `postgres-init/init.sql`

4. Right next to the postgres-init folder, create the `postgres-data` folder.

   This might get automatically created on first run, but creating it specifically is a good idea.

5. Add a reference from the database to the ApiServer project.

   In `AppHost.cs` change this:

   ```csharp
   var server = builder.AddProject<Projects.AspireAndPostgres_Server>("server")
       .WithHttpHealthCheck("/health")
       .WithExternalHttpEndpoints();
   ```

   to

   ```csharp
   var server = builder.AddProject<Projects.AspireAndPostgres_Server>("server")
       .WithHttpHealthCheck("/health")
       .WithExternalHttpEndpoints()
       .WithReference(weatherdb)
       .WaitFor(postgres);
   ```


### Add the database to the Server project

1. In the Solution Explorer, right-click on the `AspireAndPostgres.Server` project, choose `Add Aspire Component` and in the NuGet dialog, add `Aspire.Npgsql.EntityFrameworkCore.PostgreSQL` to the project.

2. In Program.cs, add this line towards the top:

    ```csharp
    builder.AddNpgsqlDbContext<WeatherDbContext>("weatherdb");
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

3. As the dashboard loads, it'll note that you don't have the username and password set.

4. Click `Set Parameters`

   - Username is `postgres`
   - Password is anything that makes you happy
   - Choose to save these in user secrets

5. Restart the project, and it should now launch successfully.

6. Open the website to ensure it works as expected.
