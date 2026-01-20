Greenfield Project from the Aspire CLI
======================================

This demo shows how to create a new Aspire project with the Aspire CLI to generate a starter project.


Steps
-----

1. Install the Aspire CLI. See https://aspire.dev/get-started/install-cli/

2. Open a terminal inside the start folder and run:

   ```sh
   aspire new
   ```

3. Choose any template that makes you happy.  The `done` folder chose `ASP.NET/Blazor`.

4. Choose other options that make you happy.

   - Redis Cache: Yes - if you have Docker Desktop or Podman running
   - Testing Project: Yes - if you'd like to experiment with testing

5. Open the project in Visual Studio, VS Code, or your favorite IDE.

6. Optional: If you chose the React starter, you need to modify `AppHost.cs` to let Node.js trust the ASP.NET development certificate:

   ```csharp
   var webfrontend = builder.AddViteApp("webfrontend", "../frontend")
    .WithEnvironment("NODE_OPTIONS", "--use-system-ca"); // make https work in dev
   ```

7. Start debugging by running this from your terminal:

   ```sh
   aspire run
   ```

   or by debugging the app from inside your IDE of choice.
