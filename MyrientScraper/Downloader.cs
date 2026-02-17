using System.Net.Http;
using System.Threading;

namespace MyrientScraper;

public class Downloader
{
    private readonly HttpClient _httpClient;

    public Downloader()
    {
        var handler = new SocketsHttpHandler
        {
            AutomaticDecompression = System.Net.DecompressionMethods.None,
            MaxConnectionsPerServer = 10,
            EnableMultipleHttp2Connections = true,
            PooledConnectionLifetime = TimeSpan.FromMinutes(5),
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1)
        };

        _httpClient = new HttpClient(handler);
        _httpClient.DefaultRequestHeaders.ConnectionClose = false;
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36");
    }

public async Task DownloadFileAsync(GameFile file, IProgress<(string, int)>? progress = null)
{
    Directory.CreateDirectory("./downloads");
    var filePath = Path.Combine("./downloads", file.Filename);

    var request = new HttpRequestMessage(HttpMethod.Get, file.Link);
    // Simula mejor un navegador real
    request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
    request.Headers.Add("Accept-Encoding", "identity"); // Sin compresión
    request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
    request.Headers.Add("Referer", new Uri(file.Link).GetLeftPart(UriPartial.Authority));

    var sw = System.Diagnostics.Stopwatch.StartNew();
    var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
    response.EnsureSuccessStatusCode();

    var totalBytes = response.Content.Headers.ContentLength ?? 0;

    await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 1048576, true);
    await using var stream = await response.Content.ReadAsStreamAsync();

    var buffer = new byte[1048576];
    var totalRead = 0L;
    var lastReported = -1;
    var lastLog = sw.ElapsedMilliseconds;

    while (true)
    {
        var read = await stream.ReadAsync(buffer);
        if (read == 0) break;

        await fileStream.WriteAsync(buffer.AsMemory(0, read));
        totalRead += read;

        // Log de velocidad cada segundo
        var now = sw.ElapsedMilliseconds;
        if (now - lastLog >= 1000)
        {
            lastLog = now;
        }

        if (totalBytes > 0)
        {
            var percent = (int)(totalRead * 100 / totalBytes);
            if (percent != lastReported)
            {
                lastReported = percent;
                progress?.Report((file.Filename, percent));
            }
        }
    }

    Console.WriteLine($"Completado en {sw.ElapsedMilliseconds}ms — {new FileInfo(filePath).Length / 1024 / 1024}MB");
}


    public long ParseSizeToBytes(string sizeStr)
    {
        sizeStr = sizeStr.Trim();

        double value;
        string unit;

        if (sizeStr.Contains(' '))
        {
            var parts = sizeStr.Split(' ');
            double.TryParse(parts[0], System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out value);
            unit = parts[1].Trim();
        }
        else
        {
            var match = System.Text.RegularExpressions.Regex.Match(sizeStr, @"^([\d.]+)([A-Za-z]+)$");
            if (match.Success)
            {
                double.TryParse(match.Groups[1].Value, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out value);
                unit = match.Groups[2].Value;
            }
            else
            {
                double.TryParse(sizeStr, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out value);
                unit = "B";
            }
        }

        long multiplier = unit.ToUpper() switch
        {
            "KB" or "KIB" => 1024,
            "MB" or "MIB" => 1024 * 1024,
            "GB" or "GIB" => 1024 * 1024 * 1024,
            "TB" or "TIB" => 1024L * 1024 * 1024 * 1024,
            _ => 1
        };

        return (long)(value * multiplier);
    }

    public string FormatBytes(long bytes)
    {
        string[] units = { "B", "KB", "MB", "GB", "TB" };
        double size = bytes;
        int unitIndex = 0;

        while (size >= 1024 && unitIndex < units.Length - 1)
        {
            size /= 1024;
            unitIndex++;
        }

        return $"{size:F1} {units[unitIndex]}";
    }
}