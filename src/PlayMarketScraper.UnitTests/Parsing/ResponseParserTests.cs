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
    public void Parse_ValidResponse_ExtractsPackageNamesInOrder()
    {
        var fakeResponse = """
            [["wrb.fr","lGYRle","[[[\"com.first.app\",7],[null,2,[512,512]]],[[[\"com.second.app\",7],[null,2,[512,512]]],[[[\"com.third.app\",7],[null,2,[512,512]]]
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
            ["com.alpha.app",7]
            ["com.beta.app",7]
            ["com.alpha.app",7]
            ["com.gamma.app",7]
            ["com.beta.app",7]
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
            ["com.app.one",7]
            ["com.app.two",7]
            ["com.app.three",7]
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
            ["com.dup.app",7]
            ["com.unique.app",7]
            ["com.dup.app",7]
            """;

        var result = _parser.Parse(fakeResponse);

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Rank, Is.EqualTo(1));
        Assert.That(result[1].Rank, Is.EqualTo(2));
    }

    [Test]
    public void Parse_NoMatches_ReturnsEmptyList()
    {
        var fakeResponse = "[\"invalid_app_name\",7] <html><body>No apps here</body></html>";

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
    public void Parse_EscapedQuotes_ExtractsPackageName()
    {
        var fakeResponse = @"[\""com.escaped.app\"",7]";

        var result = _parser.Parse(fakeResponse);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].PackageName, Is.EqualTo("com.escaped.app"));
    }
}
