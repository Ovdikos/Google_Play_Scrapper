using System.Text.RegularExpressions;
using PlayMarketScraper.Core.Interfaces;
using PlayMarketScraper.Core.Models;

namespace PlayMarketScraper.Infrastructure.Parsing;

public sealed class ResponseParser : IResponseParser
{
    private static readonly Regex PackageNameRegex = new(
        @"(?<=id(=|\\u003d))([a-zA-Z0-9_.]+)",
        RegexOptions.Compiled);

    public IReadOnlyList<PlayMarketApp> Parse(string responseBody)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var result = new List<PlayMarketApp>();

        foreach (Match match in PackageNameRegex.Matches(responseBody))
        {
            var packageName = match.Groups[2].Value;

            if (seen.Add(packageName))
            {
                result.Add(new PlayMarketApp
                {
                    PackageName = packageName,
                    Rank = result.Count + 1
                });
            }
        }

        return result;
    }
}
