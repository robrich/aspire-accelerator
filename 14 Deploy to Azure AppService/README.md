Aspire Deploy to Azure App Service
==================================

This tutorial walks through deploying to Azure App Service.  This is perfect if you'd like to deploy a few microservices to Azure without the weight of a larger orchestrator like Kubernetes.


Prerequisites
-------------

Ensure Docker Desktop or Podman is running.  You'll need this to start the containers.

Ensure the Aspire CLI is installed.  See https://aspire.dev/get-started/install-cli/ for more details.


Steps
-----

You may choose to reference the tutorial at https://www.nuget.org/packages/Aspire.Hosting.Azure.AppService/#readme-body-tab for more exhaustive steps.



### Instantiate the starter project

1. Into the `start` folder, use the Aspire Starter Template to create a project.

   If you're using the Aspire CLI, run this in the terminal:

   ```sh
   aspire new
   ```

2. Choose options that make you happy.

3. Run the project to ensure the site starts as expected and you get weather forecasts on the page.


### Add Kubernetes Deploy Details

1. In the Solution Explorer, right click on the `AppHost` project, choose `Add` -> `Aspire Component` and in the NuGet dialog, add `Aspire.Hosting.Azure.AppService`.

   **Note**: As of this writing, the package is in preview, so you'll need to check the `Pre-release` check-box in the NuGet dialog.

2. In the `AppHost` project, in `AppHost.cs`, add this line just after the builder definition:

   ```csharp
   var appServiceEnvironment = builder.AddAzureAppServiceEnvironment("appservice");
   ```

   Though we can call the environment anything we like, `appservice` sounds like a good name.  You could also call it `env` or any name you'd like.

3. Modify the Redis reference.

   Let's not run Redis as a container in Azure App Service.  Rather let's configure Azure Managed Redis instead.

   In `AppHost.cs`, change this:

   ```csharp
   var cache = builder.AddRedis("cache");
   ```

   to this:

   ```csharp
   IResourceBuilder<IResourceWithConnectionString> cache = builder.ExecutionContext.IsPublishMode
     ? builder.AddAzureManagedRedis("cache")
     : builder.AddRedis("cache");
   ```

   This say when we're deploying us Azure Managed Redis, and when we're not (e.g. locally debugging) use the Redis container.

4. In the `AppHost` project, add a NuGet reference to `Aspire.Hosting.Azure.Redis` to remove the build error.


### Debug the application

You can now debug the application from your favorite IDE or by running `aspire run` from a terminal opened in the same directory as the solution file.  The deployment details don't affect development time debugging.


### Publish the Helm chart

1. Open a terminal in the same folder as the solution file and run this:

   ```sh
   aspire do diagnostics
   ```

   The `do` command is for doing tasks like deploying.

   The `diagnostics` command lists all the steps and what each does.  This output is indeed quite verbose, but it's very helpful.

   Wander through the list of tasks you can run.

2. Build each of the containers.

   From the terminal, run this:

   ```sh
   aspire do build
   ```

   There's now a specific step for building the containers!

3. Build the Bicep files:

   From the terminal, run this:

   ```sh
   aspire do publish -o dist
   ```

   This will build the Bicep files into the `dist` folder.

4. Optional: Deploy the solution to Azure

   In the terminal run this:

   ```sh
   aspire do deploy -o dist
   ```

5. To remove the resources, go into the Azure Portal and delete the resource group you created above.
