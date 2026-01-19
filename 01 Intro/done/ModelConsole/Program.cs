using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.AI;

namespace ModelConsole;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        // Add ollama container & client to DI
        builder
            .AddOllamaApiClient("ollama")
            .AddChatClient();

        var app = builder.Build();

        // Get a reference to the chat client
        var client = app.Services.GetRequiredService<IChatClient>();

        // Configure chat client
        var chatOptions = new ChatOptions { ModelId = "llama3.2:1b" };

        // Execute query against model & display results
        var response = await client
            .GetResponseAsync("What was that song from the Rick meme that goes like `I'm never gonna...`", chatOptions);

        Console.WriteLine(response.Text);
    }
}
