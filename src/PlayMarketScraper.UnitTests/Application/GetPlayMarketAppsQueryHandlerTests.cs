using Moq;
using PlayMarketScraper.Application.Features.GetPlayMarketApps;
using PlayMarketScraper.Core.Interfaces;
using PlayMarketScraper.Core.Models;

namespace PlayMarketScraper.UnitTests.Application;

[TestFixture]
public class GetPlayMarketAppsQueryHandlerTests
{
    private Mock<IPlayMarketClient> _clientMock;
    private GetPlayMarketAppsQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _clientMock = new Mock<IPlayMarketClient>(MockBehavior.Strict);
        _handler = new GetPlayMarketAppsQueryHandler(_clientMock.Object);
    }

    [Test]
    public async Task Handle_ValidQuery_DelegatesToClientAndReturnsResult()
    {
        var expectedApps = new List<PlayMarketApp>
        {
            new() { PackageName = "com.test.app1", Rank = 1 },
            new() { PackageName = "com.test.app2", Rank = 2 }
        };

        _clientMock
            .Setup(c => c.SearchAppsAsync("poker", "us", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedApps);

        var query = new GetPlayMarketAppsQuery("poker", "us");

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.That(result, Is.SameAs(expectedApps));
    }

    [Test]
    public async Task Handle_ValidQuery_CallsSearchAppsAsyncExactlyOnce()
    {
        _clientMock
            .Setup(c => c.SearchAppsAsync("chess", "de", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PlayMarketApp>());

        var query = new GetPlayMarketAppsQuery("chess", "de");

        await _handler.Handle(query, CancellationToken.None);

        _clientMock.Verify(
            c => c.SearchAppsAsync("chess", "de", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task Handle_ValidQuery_PassesCorrectKeywordAndCountry()
    {
        string capturedKeyword = null!;
        string capturedCountry = null!;

        _clientMock
            .Setup(c => c.SearchAppsAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<string, string, CancellationToken>((kw, cc, _) =>
            {
                capturedKeyword = kw;
                capturedCountry = cc;
            })
            .ReturnsAsync(new List<PlayMarketApp>());

        var query = new GetPlayMarketAppsQuery("racing", "ru");

        await _handler.Handle(query, CancellationToken.None);

        Assert.That(capturedKeyword, Is.EqualTo("racing"));
        Assert.That(capturedCountry, Is.EqualTo("ru"));
    }

    [Test]
    public async Task Handle_ClientReturnsEmpty_ReturnsEmptyList()
    {
        _clientMock
            .Setup(c => c.SearchAppsAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PlayMarketApp>());

        var query = new GetPlayMarketAppsQuery("nonexistent", "us");

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.That(result, Is.Empty);
    }
}
