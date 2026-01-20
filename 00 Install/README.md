Upgrade Aspire
==============

If you haven't used Aspire recently or you need to upgrade to the latest, here's the steps.


Install the CLI
---------------

To install the Aspire CLI for the first time, see https://aspire.dev/get-started/install-cli/


Upgrade the CLI
---------------

To upgrade the Aspire CLI, open a terminal and run

```sh
aspire update --self
```

If you've installed Aspire as a global tool, you may instead want to uninstall the global tool and install the CLI again as directed above.

```sh
dotnet tool uninstall aspire.cli -g
```


Install or Upgrade the Project Templates
----------------------------------------

To install the Aspire New Project Templates, open a terminal and run:

```sh
dotnet new install Aspire.ProjectTemplates
```


Upgrading existing solutions to Aspire 13 and beyond
----------------------------------------------------

Upgrading from Aspire 9 or earlier to Aspire 13 and beyond is not part of the scope of this talk.  See https://learn.microsoft.com/en-us/dotnet/aspire/get-started/upgrade-to-aspire-13 for more details about how to complete this task.


Docker or Podman
----------------

Many of these tutorials use Docker containers.  You'll need either Docker Desktop (the easy button) or Podman (free and open-source) running to use containers.  See https://www.docker.com/products/docker-desktop/ or https://podman-desktop.io/ for setup instructions.
