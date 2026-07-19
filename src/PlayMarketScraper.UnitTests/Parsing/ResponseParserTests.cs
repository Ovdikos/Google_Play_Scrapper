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
        // Arrange
        var fakeResponse = """
            )]}'

            some data id=com.first.app more data
            some data id=com.second.app more data
            some data id\u003dcom.third.app more data
            """;

        // Act
        var result = _parser.Parse(fakeResponse);

        // Assert
        Assert.That(result, Has.Count.EqualTo(3));
        Assert.That(result[0].PackageName, Is.EqualTo("com.first.app"));
        Assert.That(result[1].PackageName, Is.EqualTo("com.second.app"));
        Assert.That(result[2].PackageName, Is.EqualTo("com.third.app"));
    }

    [Test]
    public void Parse_DuplicatePackages_RemovesDuplicatesPreservingFirstOccurrence()
    {
        // Arrange
        var fakeResponse = """
            )]}'

            id=com.alpha.app data id=com.beta.app data
            id=com.alpha.app data id=com.gamma.app data
            id=com.beta.app data
            """;

        // Act
        var result = _parser.Parse(fakeResponse);

        // Assert
        Assert.That(result, Has.Count.EqualTo(3));
        Assert.That(result[0].PackageName, Is.EqualTo("com.alpha.app"));
        Assert.That(result[1].PackageName, Is.EqualTo("com.beta.app"));
        Assert.That(result[2].PackageName, Is.EqualTo("com.gamma.app"));
    }

    [Test]
    public void Parse_ValidResponse_AssignsSequentialRankStartingFromOne()
    {
        // Arrange
        var fakeResponse = "id=com.app.one data id=com.app.two data id=com.app.three";

        // Act
        var result = _parser.Parse(fakeResponse);

        // Assert
        Assert.That(result[0].Rank, Is.EqualTo(1));
        Assert.That(result[1].Rank, Is.EqualTo(2));
        Assert.That(result[2].Rank, Is.EqualTo(3));
    }

    [Test]
    public void Parse_DuplicatePackages_RankSkipsDuplicates()
    {
        // Arrange — "com.dup.app" appears twice, rank should be 1, 2 (not 1, 2, 3)
        var fakeResponse = "id=com.dup.app data id=com.unique.app data id=com.dup.app";

        // Act
        var result = _parser.Parse(fakeResponse);

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Rank, Is.EqualTo(1));
        Assert.That(result[1].Rank, Is.EqualTo(2));
    }

    [Test]
    public void Parse_NoMatches_ReturnsEmptyList()
    {
        // Arrange
        var fakeResponse = ")]}'\n\nno package names here at all";

        // Act
        var result = _parser.Parse(fakeResponse);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Parse_EmptyString_ReturnsEmptyList()
    {
        // Act
        var result = _parser.Parse(string.Empty);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Parse_BothIdFormats_ExtractsFromBothEqualsAndUnicode()
    {
        // Arrange — mix of id= and id\u003d (URL-encoded '=')
        var fakeResponse = "id=com.equals.app data id\\u003dcom.unicode.app";

        // Act
        var result = _parser.Parse(fakeResponse);

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].PackageName, Is.EqualTo("com.equals.app"));
        Assert.That(result[1].PackageName, Is.EqualTo("com.unicode.app"));
    }
}
