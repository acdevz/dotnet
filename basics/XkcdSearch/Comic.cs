using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace XkcdSearch;

public record Comic()
{
    [JsonPropertyName("num")]
    public int Number { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("month")]
    public string? Month { get; init; }

    [JsonPropertyName("day")]
    public string? Day { get; init; }

    [JsonPropertyName("year")]
    public string? Year { get; init; }

    [JsonIgnore]
    public DateOnly Date
    {
        get
        {
            return DateOnly.Parse($"{Year}-{Month}-{Day}");
        }
    }

    private static readonly HttpClient client = new() { BaseAddress = new Uri("https://xkcd.com") };
    public static Comic? GetComic(int number)
    {
        try
        {
            var path = number == 0 ? "info.0.json" : $"{number}/info.0.json";
            var stream = client.GetStreamAsync(path).Result; // synchronous call !
            return JsonSerializer.Deserialize<Comic>(stream);
        }
        catch (AggregateException e) when (e.InnerException is HttpRequestException) // explain this?
        {
            return null;
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    private static async Task<Comic?> GetComicAsync(int number, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return null;
        }
        try
        {
            var path = number == 0 ? "info.0.json" : $"{number}/info.0.json";
            var stream = await client.GetStreamAsync(path, cancellationToken: token);
            return await JsonSerializer.DeserializeAsync<Comic>(stream, cancellationToken: token);
        }
        catch (AggregateException e) when (e.InnerException is HttpRequestException) // explain this?
        {
            return null;
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (TaskCanceledException)
        {
            return null;
        }
    }

    public static Comic? GetComicWithTitle(string title)
    {
        var lastComic = GetComic(0);
        for (int number = lastComic!.Number; number > 0; number--)
        {
            var comic = GetComic(number);
            if (comic != null && string.Equals(comic.Title, title, StringComparison.OrdinalIgnoreCase))
            {
                return comic;
            }
        }
        return null;
    }

    public static async Task<Comic?> GetComicWithTitleAsync(string title)
    {
        var cancellationToken = new CancellationTokenSource();
        var lastComic = await GetComicAsync(0, cancellationToken.Token);
        var tasks = new List<Task>();
        Comic? foundComic = null;
        for (int number = lastComic!.Number; number > 0; number--)
        {
            var localNumber = number; // cause number value changes in loop scope
            var comicTask = GetComicAsync(localNumber, cancellationToken.Token);
            var continuationTask = comicTask.ContinueWith(t =>
            {
                try
                {
                    var comic = t.Result;
                    if (comic != null && string.Equals(comic.Title, title, StringComparison.OrdinalIgnoreCase))
                    {
                        cancellationToken.Cancel();
                        foundComic = comic;
                    }
                }
                catch (TaskCanceledException) { }
            });
            tasks.Add(continuationTask);
        }
        await Task.WhenAll(tasks);
        return foundComic;
    }
}