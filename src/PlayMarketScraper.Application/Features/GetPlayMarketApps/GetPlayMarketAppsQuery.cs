using MediatR;
using PlayMarketScraper.Core.Models;

namespace PlayMarketScraper.Application.Features.GetPlayMarketApps;

public sealed record GetPlayMarketAppsQuery(
    string Keyword,
    string Country
) : IRequest<IReadOnlyList<PlayMarketApp>>;
