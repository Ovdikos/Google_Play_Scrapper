using PlayMarketScraper.Core.Models;

namespace PlayMarketScraper.Core.Interfaces;

public interface IResponseParser
{
    IReadOnlyList<PlayMarketApp> Parse(string responseBody);
}
