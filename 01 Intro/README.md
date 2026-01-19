# Aspire Intro Demo

This demo provides a quick introduction to **Aspire** and **Microsoft.Extensions.Hosting** for orchestrating multiple projects.

## Overview

The demo showcases how to:

- Set up an **AppHost** project that orchestrates multiple services
- Host a **Small Language Model (SLM)** using Ollama
- Create a **client application** that communicates with the hosted model to ask questions

## Folder Structure

### `done/`

Contains the **completed demo** with all code implemented. This is a fully working example that demonstrates:
- `AspireIntro.AppHost` - The orchestration layer that manages all services
- `AspireIntro.ServiceDefaults` - Shared service configuration
- `ModelConsole` - Console application that uses the hosted model

You can run this to see the full implementation in action.

### `start/`

Contains a **starting point** with the basic project structure already set up. This is ideal if you want to:

- Add the detailed code yourself
- Follow along with step-by-step instructions
- Learn by implementing the integration

## Getting Started

1. Choose either the `done` folder to run the completed demo or the `start` folder to build it yourself
2. Ensure you have Ollama installed and running with the `llama3.2:1b` model available
3. Open the `.slnx` file to load the solution
4. Run the AppHost project to orchestrate the services

## What It Does

The demo demonstrates the Rick Astley meme by asking the hosted model: "What was that song from the Rick meme that goes like `I'm never gonna...`?"

The model responds with the answer, showcasing seamless orchestration between the AppHost and the client application through Aspire.

The answer appears in the logs of the Console application. To see it, use the `Console` tab of the Aspire dashboard while the application is running but after the console app has completed.
