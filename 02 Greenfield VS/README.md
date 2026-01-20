Greenfield Project in Visual Studio
===================================

This demo shows how to create a new Aspire project in Visual Studio.  We start with the Aspire Starter Template.


Steps
-----

1. Ensure Aspire is installed and the Aspire templates are up-to-date.

2. Inside Visual Studio 2026 or later, choose File -> New Project

3. Choose the Aspire Starter Template.

4. Save the content to the start folder.

5. Choose options that make you happy.  Perhaps you choose:

   - Redis Cache: Yes - if you have Docker Desktop or Podman running
   - Testing Project: Yes - if you'd like to experiment with testing

6. Optional: If you chose the React starter, you need to modify `AppHost.cs` to let Node.js trust the ASP.NET development certificate:

   ```csharp
   var webfrontend = builder.AddViteApp("webfrontend", "../frontend")
    .WithEnvironment("NODE_OPTIONS", "--use-system-ca"); // make https work in dev
   ```

7. Start debugging the application and take a look around.
