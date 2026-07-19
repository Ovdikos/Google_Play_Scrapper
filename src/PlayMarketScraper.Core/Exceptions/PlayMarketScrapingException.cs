namespace PlayMarketScraper.Core.Exceptions;

public sealed class PlayMarketScrapingException : Exception
{
    public PlayMarketScrapingException()
    {
    }

    public PlayMarketScrapingException(string message)
        : base(message)
    {
    }

    public PlayMarketScrapingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
