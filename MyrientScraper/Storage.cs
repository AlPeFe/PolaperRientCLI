using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace MyrientScraper;

public class Storage
{
    private const string DataDir = "./scraped_data";

    public async Task<string> SaveToJsonAsync(ScrapedData data)
    {
        Directory.CreateDirectory(DataDir);

        var timestamp = data.ScrapedAt.ToString("yyyyMMdd-HHmmss");
        var filename = $"{SanitizeFilename(data.Title)}_{timestamp}.json";
        var jsonPath = Path.Combine(DataDir, filename);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(data, options);
        await File.WriteAllTextAsync(jsonPath, json);

        return jsonPath;
    }

    public async Task<ScrapedData?> LoadFromJsonAsync(string jsonPath)
    {
        var json = await File.ReadAllTextAsync(jsonPath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<ScrapedData>(json, options);
    }

    private string SanitizeFilename(string name)
    {
        return Regex.Replace(name, @"[<>:""/\\|?*]", "_").Trim().Substring(0, Math.Min(name.Length, 50));
    }
}
