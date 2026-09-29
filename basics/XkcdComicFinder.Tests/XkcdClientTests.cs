using System.Net;
using FakeItEasy;

namespace XkcdComicFinder.Tests;

public class XkcdClientTests
{
    private readonly XkcdClient xkcdClient;
    private readonly HttpMessageHandler _fakeMsgHandler;
    private const string LatestJson = """
    {
        "month": "9",
        "num": 3297,
        "link": "",
        "year": "2026",
        "news": "",
        "safe_title": "OH Scale",
        "transcript": "",
        "alt": "To access distant parts of the project, I ended up building a regular-scale passenger train inside each rail.",
        "img": "https://imgs.xkcd.com/comics/oh_scale.png",
        "title": "OH Scale",
        "day": "11"
    }
    """;

    public XkcdClientTests()
    {
        _fakeMsgHandler = A.Fake<HttpMessageHandler>();
        var httpClient = SetupHttpClient(_fakeMsgHandler);
        xkcdClient = new(httpClient);
    }

    internal static HttpClient SetupHttpClient(HttpMessageHandler messageHandler)
    {
        HttpClient httpClient = new(messageHandler)
        {
            BaseAddress = new Uri("https://xkcd.com")
        };
        return httpClient;
    }

    [Fact]
    public async Task GetLatest()
    {
        A.CallTo(_fakeMsgHandler)
            .WithReturnType<Task<HttpResponseMessage>>()
            .Where(c => c.Method.Name == "SendAsync")
            .Returns(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(LatestJson)
            });
        var comic = await xkcdClient.GetLatestAsync();
        Assert.Equal(3297, comic.Number);
    }

    [Fact]
    public async Task NoComicFound()
    {
        A.CallTo(_fakeMsgHandler)
            .WithReturnType<Task<HttpResponseMessage>>()
            .Where(c => c.Method.Name == "SendAsync")
            .Returns(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("")
            });
        var comic = await xkcdClient.GetByNumberAsync(1);
        Assert.Null(comic);
    }

    [Fact]
    public async Task GetByNumber()
    {
        A.CallTo(_fakeMsgHandler)
           .WithReturnType<Task<HttpResponseMessage>>()
           .Where(c => c.Method.Name == "SendAsync")
           .Returns(new HttpResponseMessage()
           {
               StatusCode = HttpStatusCode.OK,
               Content = new StringContent(LatestJson)
           });
        var comic = await xkcdClient.GetByNumberAsync(3297);
        Assert.NotNull(comic);
        Assert.Equal(3297, comic.Number);
    }
}