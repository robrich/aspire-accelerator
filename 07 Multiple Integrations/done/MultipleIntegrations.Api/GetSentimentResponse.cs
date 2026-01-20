namespace MultipleIntegrations.Api;

public class GetSentimentResponse(IEnumerable<ScoredLabel> labels, string summary)
{
    public IEnumerable<ScoredLabel> Labels { get; set; } = labels;
    public string Summary { get; set; } = summary;
}
