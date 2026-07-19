using MediatR;
using PlayMarketScraper.Core.Interfaces;
using PlayMarketScraper.Core.Models;

namespace PlayMarketScraper.Application.Features.GetPlayMarketApps;

public sealed class GetPlayMarketAppsQueryHandler
    : IRequestHandler<GetPlayMarketAppsQuery, IReadOnlyList<PlayMarketApp>>
{
    private readonly IPlayMarketClient _client;

    public GetPlayMarketAppsQueryHandler(IPlayMarketClient client)
        => _client = client;

    public async Task<IReadOnlyList<PlayMarketApp>> Handle(
        GetPlayMarketAppsQuery request,
        CancellationToken cancellationToken)
    {
        return await _client.SearchAppsAsync(
            request.Keyword,
            request.Country,
            cancellationToken);
    }
}
