using Spectre.Console;

namespace MyrientScraper;

public class UserInterface
{
    private readonly Scraper _scraper;
    private readonly Storage _storage;
    private readonly Downloader _downloader;

    public UserInterface()
    {
        _scraper = new Scraper();
        _storage = new Storage();
        _downloader = new Downloader();
    }

    public async Task RunAsync()
    {
        await ShowMainMenuAsync();
    }

    private async Task ShowMainMenuAsync()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new FigletText("Myrient Downloader")
                    .Centered()
                    .Color(Color.Magenta));

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Select an option:[/]")
                    .AddChoices(new[] { "Scrape URL", "Load JSON", "Quit" }));

            switch (choice)
            {
                case "Scrape URL":
                    await ScrapeAndSelectFilesAsync();
                    break;
                case "Load JSON":
                    await LoadAndSelectFilesAsync();
                    break;
                case "Quit":
                    return;
            }
        }
    }

    private async Task ScrapeAndSelectFilesAsync()
    {
        var url = AnsiConsole.Ask<string>("[green]Enter URL:[/]");
        AnsiConsole.WriteLine();

        ScrapedData? data = null;

        await AnsiConsole.Status()
            .StartAsync("[yellow]Scraping...[/]", async ctx =>
            {
                data = await _scraper.ScrapeMyrientAsync(url);
                var jsonPath = await _storage.SaveToJsonAsync(data);
                ctx.Status($"[green]Scraped {data.Files.Count} files[/]");
                ctx.Status($"[green]Saved to: {jsonPath}[/]");
                Thread.Sleep(1500);
            });

        if (data != null && data.Files.Count > 0)
        {
            await ShowFilesAndSelectAsync(data);
        }
    }

    private async Task LoadAndSelectFilesAsync()
    {
        var jsonDir = "./scraped_data";
        if (!Directory.Exists(jsonDir))
        {
            AnsiConsole.MarkupLine("[red]No saved JSON files found[/]");
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            Console.ReadKey();
            return;
        }

        var jsonFiles = Directory.GetFiles(jsonDir, "*.json")
            .OrderByDescending(f => File.GetCreationTime(f))
            .ToList();

        if (jsonFiles.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No saved JSON files found[/]");
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            Console.ReadKey();
            return;
        }

        var selectedJson = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Select JSON file:[/]")
                .PageSize(10)
                .AddChoices(jsonFiles.Select(Path.GetFileName)));

        var jsonPath = Path.Combine(jsonDir, selectedJson);
        var data = await _storage.LoadFromJsonAsync(jsonPath);

        if (data != null)
        {
            await ShowFilesAndSelectAsync(data);
        }
    }

    private async Task ShowFilesAndSelectAsync(ScrapedData data)
    {
        var filterText = string.Empty;

        while (true)
        {
            AnsiConsole.Clear();

            AnsiConsole.Write(
                new Rule(data.Title)
                    .RuleStyle(Style.Parse("magenta"))
                    .Centered());

            var filesToDisplay = data.Files;

            if (!string.IsNullOrEmpty(filterText))
            {
                filesToDisplay = data.Files
                    .Where(f => f.Filename.Contains(filterText, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                AnsiConsole.MarkupLine($"[yellow]Filter: \"{Markup.Escape(filterText)}\"[/]");
                AnsiConsole.MarkupLine($"[yellow]Showing {filesToDisplay.Count} of {data.Files.Count} files[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[yellow]Showing all {data.Files.Count} files[/]");
            }
            AnsiConsole.WriteLine();

            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]What would you like to do?[/]")
                    .AddChoices(new[] { "Select files to download", "Filter files", "Clear filter", "Back to main menu" }));

            switch (action)
            {
                case "Filter files":
                    var newFilter = AnsiConsole.Ask<string>("[green]Enter filter text (or leave empty to cancel):[/]");
                    if (!string.IsNullOrWhiteSpace(newFilter))
                        filterText = newFilter;
                    continue;
                case "Clear filter":
                    filterText = string.Empty;
                    continue;
                case "Back to main menu":
                    return;
            }

            var filesWithIndex = filesToDisplay
                .Select((f, i) => (Index: data.Files.IndexOf(f), File: f))
                .ToList();

            // Crear lista de opciones de selección como strings
            var choiceOptions = new List<string> { "[red]← BACK TO CHANGE FILTER[/]" };
            foreach (var f in filesWithIndex)
            {
                choiceOptions.Add($"{f.Index + 1}. {f.File.Filename.Truncate(50)} ({f.File.Size})");
            }

            var selectedChoices = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title("[green]Select files to download:[/]")
                    .NotRequired()
                    .PageSize(20)
                    .AddChoices(choiceOptions));

            // Verificar si el usuario seleccionó la opción de volver
            if (selectedChoices.Any(c => c.Contains("BACK TO CHANGE FILTER")))
            {
                continue; // Volver al menú de filtros
            }

            // Procesar archivos seleccionados
            var selectedFiles = new List<(int Index, GameFile File)>();
            foreach (var choice in selectedChoices)
            {
                if (choice.Contains("BACK"))
                    continue;

                var parts = choice.Split(new[] { ". ", " (" }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                {
                    var index = int.Parse(parts[0].Trim()) - 1;
                    var file = data.Files[index];
                    selectedFiles.Add((index, file));
                }
            }

            if (selectedFiles.Count == 0)
            {
                var backAction = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]What would you like to do?[/]")
                        .AddChoices(new[] { "Filter files", "Clear filter", "Back to main menu" }));

                switch (backAction)
                {
                    case "Filter files":
                        var newFilter = AnsiConsole.Ask<string>("[green]Enter filter text (or leave empty to cancel):[/]");
                        if (!string.IsNullOrWhiteSpace(newFilter))
                            filterText = newFilter;
                        continue;
                    case "Clear filter":
                        filterText = string.Empty;
                        continue;
                    case "Back to main menu":
                        return;
                }
                continue;
            }

            var confirmAction = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[green]Selected {selectedFiles.Count} files. What would you like to do?[/]")
                    .AddChoices(new[] { "Download", "Filter more", "Clear filter", "Back to selection", "Cancel and go back" }));

            switch (confirmAction)
            {
                case "Download":
                    await DownloadFilesAsync(data, selectedFiles);
                    return;
                case "Filter more":
                    var newFilter = AnsiConsole.Ask<string>("[green]Enter filter text (or leave empty to cancel):[/]");
                    if (!string.IsNullOrWhiteSpace(newFilter))
                        filterText = newFilter;
                    continue;
                case "Clear filter":
                    filterText = string.Empty;
                    continue;
                case "Back to selection":
                    continue;
                case "Cancel and go back":
                    continue;
            }

            await DownloadFilesAsync(data, selectedFiles);
            return;
        }
    }

    private async Task DownloadFilesAsync(ScrapedData data, List<(int Index, GameFile File)> selectedFiles)
    {
        var totalSize = selectedFiles.Sum(f => _downloader.ParseSizeToBytes(f.File.Size));

        AnsiConsole.Clear();
        AnsiConsole.Write(
            new FigletText("Download Summary")
                .Centered()
                .Color(Color.Green));

        AnsiConsole.MarkupLine($"\n[green]Files: {selectedFiles.Count}[/]");
        AnsiConsole.MarkupLine($"[green]Total size: {_downloader.FormatBytes(totalSize)}[/]\n");

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Blue)
            .AddColumn(new TableColumn("[yellow]#[/]").Centered())
            .AddColumn(new TableColumn("[yellow]Filename[/]"))
            .AddColumn(new TableColumn("[yellow]Size[/]").Centered());

        for (int i = 0; i < Math.Min(10, selectedFiles.Count); i++)
        {
            var file = selectedFiles[i].File;
            table.AddRow((i + 1).ToString(), file.Filename.Truncate(40), file.Size);
        }

        if (selectedFiles.Count > 10)
        {
            table.AddRow("", $"... and {selectedFiles.Count - 10} more files", "");
        }

        AnsiConsole.Write(table);

        if (!AnsiConsole.Confirm("\n[yellow]Proceed with download?[/]"))
        {
            AnsiConsole.MarkupLine("\n[red]Download cancelled.[/]");
            return;
        }

        AnsiConsole.MarkupLine("\n[green]Starting download...[/]\n");

        await AnsiConsole.Progress()
            .StartAsync(async ctx =>
            {
                var count = 0;
                var total = selectedFiles.Count;

                foreach (var selected in selectedFiles)
                {
                    count++;
                    var file = selected.File;
                    var task = ctx.AddTask($"{count}/{total} {Markup.Escape(file.Filename).Truncate(50)}", true, total);

                    try
                    {
                        var progress = new Progress<(string, int)>(update =>
                        {
                            task.Value = update.Item2;
                        });

                        // Descarga simple con 1MB buffer y AutomaticDecompression = None
                        await _downloader.DownloadFileAsync(file, progress);

                        task.Value = 100;
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.MarkupLine($"[red]Error downloading {file.Filename}: {ex.Message}[/]");
                    }

                    task.Increment(1);
                }
            });

        AnsiConsole.MarkupLine("\n[green]✓ Download complete![/]");

        if (AnsiConsole.Confirm("\n[yellow]Download another set?[/]"))
        {
            await ShowFilesAndSelectAsync(data);
        }
    }
}
