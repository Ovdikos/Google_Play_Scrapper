using System.Text.RegularExpressions;
using PlayMarketScraper.Core.Interfaces;
using PlayMarketScraper.Core.Models;

namespace PlayMarketScraper.Infrastructure.Parsing;

public sealed class ResponseParser : IResponseParser
{
    private static readonly Regex PackageNameRegex = new(
        @"\[\\?""([a-zA-Z0-9_.-]+)\\?"",\s*\d+\]",
        RegexOptions.Compiled);

    public IReadOnlyList<PlayMarketApp> Parse(string responseBody)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var result = new List<PlayMarketApp>();

        foreach (Match match in PackageNameRegex.Matches(responseBody))
        {
            var packageName = match.Groups[1].Value;

            if (packageName.Contains('.') && !packageName.Contains(' '))
            {
                if (seen.Add(packageName))
                {
                    result.Add(new PlayMarketApp
                    {
                        PackageName = packageName,
                        Rank = result.Count + 1
                    });
                }
            }
        }

        return result;
    }
}
