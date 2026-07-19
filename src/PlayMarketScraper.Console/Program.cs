using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PlayMarketScraper.Application;
using PlayMarketScraper.Application.Features.GetPlayMarketApps;
using PlayMarketScraper.Core.Exceptions;
using PlayMarketScraper.Infrastructure;
using Spectre.Console;

// ──────────────────────────────────────────────
// 1. Composition Root — wire all layers via DI
// ──────────────────────────────────────────────
var services = new ServiceCollection()
    .AddApplication()
    .AddInfrastructure()
    .BuildServiceProvider();

var mediator = services.GetRequiredService<IMediator>();

// ──────────────────────────────────────────────
// 2. Welcome Banner
// ──────────────────────────────────────────────
AnsiConsole.Write(
    new FigletText("Play Scraper")
        .Centered()
        .Color(Color.Green));

AnsiConsole.Write(
    new Rule("[dim]Google Play Store Package Name Extractor[/]")
        .RuleStyle("green dim"));

AnsiConsole.WriteLine();

// ──────────────────────────────────────────────
// 3. User Input
// ──────────────────────────────────────────────
var keyword = AnsiConsole.Ask<string>("[green]Search keyword:[/]");
var country = AnsiConsole.Ask<string>("[green]Country code[/] [dim](e.g. us, ru, de):[/]");

// ──────────────────────────────────────────────
// 4. Execute Query with Spinner
// ──────────────────────────────────────────────
try
{
    var apps = await AnsiConsole.Status()
        .AutoRefresh(true)
        .Spinner(Spinner.Known.Dots)
        .SpinnerStyle(Style.Parse("green bold"))
        .StartAsync("Fetching apps from Google Play...", async _ =>
        {
            return await mediator.Send(new GetPlayMarketAppsQuery(keyword, country));
        });

    // ──────────────────────────────────────────
    // 5. Display Results
    // ──────────────────────────────────────────
    AnsiConsole.WriteLine();

    if (apps.Count == 0)
    {
        AnsiConsole.MarkupLine("[yellow]⚠ No apps found for the given keyword.[/]");
        return;
    }

    var table = new Table()
        .Border(TableBorder.Rounded)
        .Title($"[bold green]Results for \"{keyword}\" ({country.ToUpperInvariant()})[/]")
        .AddColumn(new TableColumn("[bold]Rank[/]").Centered())
        .AddColumn(new TableColumn("[bold]Package Name[/]"));

    foreach (var app in apps)
    {
        table.AddRow(
            $"[cyan]{app.Rank}[/]",
            $"[white]{app.PackageName}[/]");
    }

    AnsiConsole.Write(table);
    AnsiConsole.MarkupLine($"\n[green]✓ Total:[/] {apps.Count} unique apps found.");
}
catch (PlayMarketScrapingException ex)
{
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[red bold]✗ Scraping Error[/]");
    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
}
catch (Exception ex)
{
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[red bold]✗ Unexpected Error[/]");
    AnsiConsole.WriteException(ex);
}