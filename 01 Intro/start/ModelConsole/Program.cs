using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.AI;

namespace ModelConsole;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        // TODO: Add ollama container & client to DI

        var app = builder.Build();

        // TODO: Get a reference to the chat client

        // TODO: Configure chat client

        // TODO: Execute query against model & display results

    }
}
