using System.Net;
using System.Text.Json;
using FakeItEasy;
using Microsoft.Data.Sqlite;

namespace XkcdComicFinder.Tests;

public class ComicFinderTests : IDisposable
{
    private const string NumberLink = "https://xkcd.com/{0}/info.0.json";
    private const string LatestLink = "https://xkcd.com/info.0.json";
    private readonly ComicDbContext _comicDbContext;
    private readonly SqliteConnection _keepAliveConn;
    private readonly HttpMessageHandler _fakeMsgHandler;
    private readonly ComicFinder _comicFinder;

    public ComicFinderTests()
    {
        (_comicDbContext, _keepAliveConn) = ComicRepositoryTests.SetupSqlite("comics_int");
        var _comicRepo = new ComicRepository(_comicDbContext);
        _fakeMsgHandler = A.Fake<HttpMessageHandler>();
        var httpClient = XkcdClientTests.SetupHttpClient(_fakeMsgHandler);
        var xkcdClient = new XkcdClient(httpClient);
        _comicFinder = new(xkcdClient, _comicRepo);
    }

    private static Uri GetUri(Comic c) => new Uri(string.Format(NumberLink, c.Number));

    [Fact]
    public async Task StartsWithEmptyRepo()
    {
        var comics = new[]
        {
            new Comic(){Number = 12, Title = "b"},
            new Comic(){Number = 1, Title = "a"},
            new Comic(){Number = 4, Title = "c"}
        };
        var responses = comics.ToDictionary(GetUri, c => JsonSerializer.Serialize(c));
        responses.Add(new Uri(LatestLink), JsonSerializer.Serialize(comics[0]));

        A.CallTo(_fakeMsgHandler)
            .WithReturnType<Task<HttpResponseMessage>>()
            .Where(c => c.Method.Name == "SendAsync")
            .Returns(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.NotFound
            });

        foreach (var responsePair in responses)
        {
            A.CallTo(_fakeMsgHandler)
                .WithReturnType<Task<HttpResponseMessage>>()
                .Where(c => c.Method.Name == "SendAsync")
                .WhenArgumentsMatch(args =>
                    args.First() is HttpRequestMessage req && req.RequestUri == responsePair.Key)
                .Returns(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responsePair.Value)
                });
        }

        var foundComics = (await _comicFinder.FindAsync("b")).ToBlockingEnumerable();
        Assert.Single(foundComics);
        Assert.Single(foundComics, c => c.Number == 12);
    }

    public void Dispose()
    {
        _keepAliveConn.Close();
        _comicDbContext.Dispose();
    }
}