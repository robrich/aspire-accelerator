using Microsoft.AspNetCore.Mvc;

namespace MultipleIntegrations.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SentimentController : ControllerBase
{
    [HttpGet(Name = "GetSentiment")]
    public async Task<GetSentimentResponse> Get(string value)
    {
        // TODO: Implement sentiment analysis logic here.
        return new GetSentimentResponse([], $"The sentiment of {value} is...");
    }
}
