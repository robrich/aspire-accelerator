namespace MultipleIntegrations.Api;

public class ScoredLabel(string value, float score)
{
    public string Value { get; set; } = value;
    public float Score { get; set; } = score;
}
