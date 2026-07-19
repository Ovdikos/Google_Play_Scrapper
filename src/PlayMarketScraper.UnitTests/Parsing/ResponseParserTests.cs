using PlayMarketScraper.Infrastructure.Parsing;

namespace PlayMarketScraper.UnitTests.Parsing;

[TestFixture]
public class ResponseParserTests
{
    private ResponseParser _parser;

    [SetUp]
    public void SetUp()
    {
        _parser = new ResponseParser();
    }

    [Test]
    public void Parse_ValidHtmlResponse_ExtractsPackageNamesInOrder()
    {
        var fakeResponse = """
            <a href="/store/apps/details?id=com.first.app">App 1</a>
            <a href="/store/apps/details?id=com.second.app">App 2</a>
            <a href="/store/apps/details?id=com.third.app">App 3</a>
            """;

        var result = _parser.Parse(fakeResponse);

        Assert.That(result, Has.Count.EqualTo(3));
        Assert.That(result[0].PackageName, Is.EqualTo("com.first.app"));
        Assert.That(result[1].PackageName, Is.EqualTo("com.second.app"));
        Assert.That(result[2].PackageName, Is.EqualTo("com.third.app"));
    }

    [Test]
    public void Parse_DuplicatePackages_RemovesDuplicatesPreservingFirstOccurrence()
    {
        var fakeResponse = """
            <a href="/store/apps/details?id=com.alpha.app"></a>
            <a href="/store/apps/details?id=com.beta.app"></a>
            <a href="/store/apps/details?id=com.alpha.app"></a>
            <a href="/store/apps/details?id=com.gamma.app"></a>
            <a href="/store/apps/details?id=com.beta.app"></a>
            """;

        var result = _parser.Parse(fakeResponse);

        Assert.That(result, Has.Count.EqualTo(3));
        Assert.That(result[0].PackageName, Is.EqualTo("com.alpha.app"));
        Assert.That(result[1].PackageName, Is.EqualTo("com.beta.app"));
        Assert.That(result[2].PackageName, Is.EqualTo("com.gamma.app"));
    }

    [Test]
    public void Parse_ValidResponse_AssignsSequentialRankStartingFromOne()
    {
        var fakeResponse = """
            /store/apps/details?id=com.app.one
            /store/apps/details?id=com.app.two
            /store/apps/details?id=com.app.three
            """;

        var result = _parser.Parse(fakeResponse);

        Assert.That(result[0].Rank, Is.EqualTo(1));
        Assert.That(result[1].Rank, Is.EqualTo(2));
        Assert.That(result[2].Rank, Is.EqualTo(3));
    }

    [Test]
    public void Parse_DuplicatePackages_RankSkipsDuplicates()
    {
        var fakeResponse = """
            /store/apps/details?id=com.dup.app
            /store/apps/details?id=com.unique.app
            /store/apps/details?id=com.dup.app
            """;

        var result = _parser.Parse(fakeResponse);

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Rank, Is.EqualTo(1));
        Assert.That(result[1].Rank, Is.EqualTo(2));
    }

    [Test]
    public void Parse_NoMatches_ReturnsEmptyList()
    {
        var fakeResponse = "<html><body>No apps here</body></html>";

        var result = _parser.Parse(fakeResponse);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Parse_EmptyString_ReturnsEmptyList()
    {
        var result = _parser.Parse(string.Empty);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Parse_UnicodeEncodedEquals_ExtractsPackageName()
    {
        var fakeResponse = @"/store/apps/details?id\u003dcom.unicode.app";

        var result = _parser.Parse(fakeResponse);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].PackageName, Is.EqualTo("com.unicode.app"));
    }
}
