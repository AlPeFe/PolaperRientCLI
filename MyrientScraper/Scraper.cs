using AngleSharp;
using AngleSharp.Html.Parser;
using System.Text.RegularExpressions;

namespace MyrientScraper;

public class Scraper
{
    private readonly HttpClient _httpClient;

    public Scraper()
    {
        _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
    }

    public async Task<ScrapedData> ScrapeMyrientAsync(string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var html = await response.Content.ReadAsStringAsync();

        var parser = new HtmlParser();
        var document = await parser.ParseDocumentAsync(html);

        var files = new List<GameFile>();
        var rows = document.QuerySelectorAll("#list tbody tr");

        foreach (var row in rows)
        {
            var sizeCell = row.QuerySelector("td.size");
            if (sizeCell == null || sizeCell.TextContent.Trim() == "-")
                continue;

            var linkCell = row.QuerySelector("td.link");
            if (linkCell == null)
                continue;

            var filename = linkCell.TextContent.Trim()
                .Replace("[UP] ", "")
                .Trim();

            if (string.IsNullOrEmpty(filename))
                continue;

            var linkElement = linkCell.QuerySelector("a");
            if (linkElement == null)
                continue;

            var link = linkElement.GetAttribute("href") ?? string.Empty;
            var date = row.QuerySelector("td.date")?.TextContent.Trim() ?? string.Empty;

            if (!link.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                if (link.StartsWith("/"))
                    link = GetBaseUrl(url) + link;
                else
                    link = url.TrimEnd('/') + "/" + link;
            }

            files.Add(new GameFile
            {
                Filename = filename,
                Link = link,
                Size = sizeCell.TextContent.Trim(),
                Date = date
            });
        }

        var title = document.QuerySelector("title")?.TextContent ?? "No-Intro Collection";

        return new ScrapedData
        {
            Url = url,
            Title = title,
            Files = files,
            ScrapedAt = DateTime.Now
        };
    }

    private string GetBaseUrl(string url)
    {
        var match = Regex.Match(url, @"^(https?://[^/]+)");
        return match.Success ? match.Groups[1].Value : "https://myrient.erista.me";
    }
}
