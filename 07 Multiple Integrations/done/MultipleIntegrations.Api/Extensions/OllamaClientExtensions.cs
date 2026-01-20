using Microsoft.Extensions.AI;

namespace MultipleIntegrations.Api.Extensions;

public static class OllamaClientExtensions
{
    public static ChatOptions OllamaChatOptions 
        => new ChatOptions
        {
            ModelId = "llama3.2:1b",
            Temperature = 0.7f,
            TopP = 0.95f,
            FrequencyPenalty = 0.0f,
            PresencePenalty = 0.0f
        };

    public static EmbeddingGenerationOptions OllamaEmbeddingOptions 
        => new EmbeddingGenerationOptions
        {
            ModelId = "nomic-embed-text"
        };

    public async static Task<float[]> GetEmbeddingAsync(this IEmbeddingGenerator<string, Embedding<float>> embeddingClient, string value) 
        => (await embeddingClient.GenerateVectorAsync(value, OllamaEmbeddingOptions)).ToArray();

    public async static Task<string> GetSentimentSummaryAsync(this IChatClient chatClient, IEnumerable<string> labels)
    {
        var chatMessages = new List<ChatMessage>
        {
            { new ChatMessage(ChatRole.System, "You are an assistant that summarizes the emotional tone of a message based on a set of predefined sentiment labels. \r\nYou do not invent new labels, reinterpret labels, or question their correctness. \r\nYour task is to produce a short, coherent summary that explains the emotional character implied by the supplied labels.\r\n\r\nGuidelines:\r\n- Use only the labels provided.\r\n- Combine the labels into a single, natural-language description.\r\n- Do not restate the original message.\r\n- Do not output the labels as a list, or expose them in your response; express them as a unified emotional summary.\r\n- Keep the summary concise, typically 1–2 sentences.") },
            { new ChatMessage(ChatRole.User, $"Given the following sentiment labels: '{string.Join(", ", labels)}' Summarize the emotional tone of the message that was tagged with these labels.")  }
        };

        var response = await chatClient.GetResponseAsync(
            chatMessages,
            OllamaChatOptions);

        return response.Text;
    }
}
