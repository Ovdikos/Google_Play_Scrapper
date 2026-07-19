using Microsoft.Extensions.DependencyInjection;
using PlayMarketScraper.Core.Interfaces;
using PlayMarketScraper.Infrastructure.Http;
using PlayMarketScraper.Infrastructure.Parsing;

namespace PlayMarketScraper.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IResponseParser, ResponseParser>();

        services.AddHttpClient<IPlayMarketClient, PlayMarketClient>(client =>
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36");
        });

        return services;
    }
}
