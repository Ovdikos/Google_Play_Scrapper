namespace PlayMarketScraper.Core.Models;

public sealed record PlayMarketApp
{
    public required string PackageName { get; init; }
    public required int Rank { get; init; }
}
