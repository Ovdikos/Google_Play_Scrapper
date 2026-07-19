# Google Play Market Scraper

A modern, fast, and robust .NET 10 Console Application built with Clean Architecture that scrapes Android application package names from the Google Play Store search results.

It strictly uses the internal Google Play `batchexecute` API via HTTP POST requests instead of parsing HTML, providing fast and reliable extraction.

## Prerequisites
* [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or higher

## Architecture
The project follows Clean Architecture principles:
* **Core:** Contains domain entities, exceptions, and interfaces.
* **Application:** Implements CQRS using MediatR for request handling.
* **Infrastructure:** Handles the HTTP communication and Regex-based response parsing.
* **Console:** The presentation layer, featuring a beautiful UI built with `Spectre.Console`.

## How to Run
1. Open a terminal (PowerShell, CMD, or bash).
2. Navigate to the root directory of the project.
3. Run the following command:
   ```bash
   dotnet run --project src/PlayMarketScraper.Console
   ```
4. Follow the on-screen prompts:
   * Enter the **search keyword** (e.g., `poker`, `chess`).
   * Enter the **country code** (e.g., `us`, `de`, `ua`, `pl`).

The application will display the results in a formatted table in the console.

## Running Tests
To run the unit tests and verify the parsing logic:
```bash
dotnet test
```
