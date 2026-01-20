namespace MultipleIntegrations.Api.Extensions;

public static class ScoredLabelExtensions
{
    public static float MinOutlierScore(this IEnumerable<ScoredLabel> labels)
    {
        return !labels.Any()
            ? 0.0f
            : labels.Mean() + (2.0f * labels.StdDev());
    }

    public static float Mean(this IEnumerable<ScoredLabel> labels)
    {
        return !labels.Any() 
            ? 0.0f 
            : labels.Average(l => l.Score);
    }

    public static float StdDev(this IEnumerable<ScoredLabel> labels)
    {
        if (!labels.Any()) return 0.0f;
        var avg = labels.Average(l => l.Score);
        var sumOfSquaresOfDifferences = labels.Select(l => (l.Score - avg) * (l.Score - avg)).Sum();
        var stdDev = (float)Math.Sqrt(sumOfSquaresOfDifferences / labels.Count());
        return stdDev;
    }
}
