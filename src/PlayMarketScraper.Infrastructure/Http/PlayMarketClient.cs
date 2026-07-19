using PlayMarketScraper.Core.Exceptions;
using PlayMarketScraper.Core.Interfaces;
using PlayMarketScraper.Core.Models;

namespace PlayMarketScraper.Infrastructure.Http;

public sealed class PlayMarketClient : IPlayMarketClient
{
    private const string EndpointTemplate =
        "https://play.google.com/_/PlayStoreUi/data/batchexecute?rpcids=lGYRle&hl=en-US&gl={0}";

    private const string PayloadTemplate =
        """[[["lGYRle","[[null,[[10,[10,50]],true,null,[96,27,4,8,57,30,110,79,11,16,49,1,3,9,12,104,55,56,51,10,34,77]],[[null,null,null,[[[[7,31],[[1,52,43,112,92,58,69,31,19,96]]]]]]],null,[null,null,null,[[[[17,31],[[1,52,43,112,92,58,69,31,19,96]]]]]]]],[\"KEYWORD_PLACEHOLDER\"],4,[null,1],null,null,[],[1]]",null,"generic"]]]""";

    private readonly HttpClient _httpClient;
    private readonly IResponseParser _parser;

    public PlayMarketClient(HttpClient httpClient, IResponseParser parser)
    {
        _httpClient = httpClient;
        _parser = parser;
    }

    public async Task<IReadOnlyList<PlayMarketApp>> SearchAppsAsync(
        string keyword,
        string country,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var url = string.Format(EndpointTemplate, country);
            var payload = PayloadTemplate.Replace("KEYWORD_PLACEHOLDER", keyword);

            using var content = new FormUrlEncodedContent(
                [new KeyValuePair<string, string>("f.req", payload)]);

            using var response = await _httpClient.PostAsync(url, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            return _parser.Parse(body);
        }
        catch (HttpRequestException ex)
        {
            throw new PlayMarketScrapingException(
                $"HTTP request failed for keyword '{keyword}', country '{country}': {ex.Message}", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new PlayMarketScrapingException(
                "The request to Google Play timed out.", ex);
        }
    }
}
