namespace MyrientScraper;

public class GameFile
{
    public string Filename { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
}

public class ScrapedData
{
    public string Url { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<GameFile> Files { get; set; } = new();
    public DateTime ScrapedAt { get; set; } = DateTime.Now;
}
