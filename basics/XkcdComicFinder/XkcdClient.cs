using System.Text.Json;

namespace XkcdComicFinder;

public class XkcdClient : IXkcdClient
{
    private const string PageUri = "info.0.json";
    private readonly HttpClient _httpClient;

    public XkcdClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Comic> GetLatestAsync()
    {
        var stream = await _httpClient.GetStreamAsync(PageUri);
        return JsonSerializer.Deserialize<Comic>(stream)!;
    }

    public async Task<Comic?> GetByNumberAsync(int number)
    {
        try
        {
            var path = $"{number}/{PageUri}";
            var stream = await _httpClient.GetStreamAsync(path);
            return JsonSerializer.Deserialize<Comic>(stream);
        }
        /* aggregate exceptions are not needed here! */
        // catch (AggregateException e) when (e.InnerException is HttpRequestException)
        // {
        //     return null;
        // }
        catch (HttpRequestException)
        {
            return null;
        }
    }

}