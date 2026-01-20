Aspire Deploy to Kubernetes
===========================

This tutorial walks through deploying to a Kubernetes Helm chart.  This is perfect if you'd like to deploy to Azure Kubernetes Service or any Kubernetes environment.


Prerequisites
-------------

Ensure Docker Desktop or Podman is running.  You'll need this to start the containers.

Ensure the Aspire CLI is installed.  See https://aspire.dev/get-started/install-cli/ for more details.


Steps
-----

You may choose to reference the tutorial at https://www.nuget.org/packages/Aspire.Hosting.Kubernetes/#readme-body-tab for more exhaustive steps.



### Instantiate the starter project

1. Into the `start` folder, use the Aspire Starter Template to create a project.

   If you're using the Aspire CLI, run this in the terminal:

   ```sh
   aspire new
   ```

2. Choose options that make you happy.

3. Run the project to ensure the site starts as expected and you get weather forecasts on the page.


### Add Kubernetes Deploy Details

1. In the Solution Explorer, right click on the `AppHost` project, choose `Add` -> `Aspire Component` and in the NuGet dialog, add `Aspire.Hosting.Kubernetes`.

   **Note**: As of this writing, the package is in preview, so you'll need to check the `Pre-release` check-box in the NuGet dialog.

2. In the `AppHost` project, in `AppHost.cs`, add this line just after the builder definition:

   ```csharp
   builder.AddKubernetesEnvironment("k8s");
   ```

   Though we can call the environment anything we like, `k8s` sounds like a good name.  You could also call it `env` or any name you'd like.


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

   In this case, because we're using the React frontend, it built the React app, and tucked the static files into the Server's wwwroot folder.  All this happens automatically!

3. Build the Helm chart:

   From the terminal, run this:

   ```sh
   aspire do publish-k8s -o dist
   ```

   This will build the Helm chart into the `dist` folder.

4. Optional: Deploy the Helm chart to Kubernetes

   Switch the terminal to the `dist` folder and run:

   ```sh
   helm install
   ```

   Alternatively, you can let Aspire do this for you:

   ```sh
   aspire do deploy -o dist
   ```

5. To remove the Helm chart, open the terminal and run this:

   ```sh
   helm ls
   helm uninstall {your-app}
   ```
