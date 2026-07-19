using PlayMarketScraper.Core.Models;

namespace PlayMarketScraper.Core.Interfaces;

public interface IPlayMarketClient
{
    Task<IReadOnlyList<PlayMarketApp>> SearchAppsAsync(
        string keyword,
        string country,
        CancellationToken cancellationToken = default);
}
