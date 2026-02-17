using Spectre.Console;

namespace MyrientScraper;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            var ui = new UserInterface();
            await ui.RunAsync();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
        }
    }
}
