using Qdrant.Client.Grpc;

namespace MultipleIntegrations.Api;

public class Sentiment
{
    public Guid Id { get; set; }
    public string Label { get; set; }
    public string Value => $"Sentiment of: {this.Label}";

    public Sentiment(string label)
    {
        this.Id = Guid.NewGuid();
        this.Label = label;
    }

    internal PointStruct AsPointStruct(float[] vector) => new PointStruct
    {
        Id = this.Id,
        Vectors = vector,
        Payload = 
        {
            { "label", new Value { StringValue = this.Label } },
            { "value", new Value { StringValue = this.Value } }
        }
    };
}
