using System.Net;
using System.Text.Json;
using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using XkcdComicService;

namespace XkcdComicFinder.Tests;

public class SearchControllerTests
{
    private const string BaseAddress = "https://xkcd.com";

    /* bootstrapping an application in memory */
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpMessageHandler _fakeMsgHandler;
    public SearchControllerTests()
    {
        _fakeMsgHandler = A.Fake<HttpMessageHandler>();
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                /* Use specific configurations for specific testing */
                // builder.UseEnvironment("Integegration"); // for appsettings.Integration.json
                builder.ConfigureServices(services =>
                {
                    ServiceDescriptor sd = services.First(
                        s => s.ServiceType == typeof(HttpClient)
                    );
                    services.Remove(sd);
                    services.AddHttpClient<IXkcdClient, XkcdClient>(
                        h => h.BaseAddress = new Uri(BaseAddress))
                        .ConfigurePrimaryHttpMessageHandler(
                            () => _fakeMsgHandler
                        );
                });
            });
    }

    private const string NumberLink = "https://xkcd.com/{0}/info.0.json";
    private const string LatestLink = "https://xkcd.com/info.0.json";
    private static Uri GetUri(Comic c) => new Uri(string.Format(NumberLink, c.Number));

    [Fact]
    public async Task FoundB()
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

        /* Now test the MVC as if an external API via HttpClient */

        HttpClient client = _factory.CreateClient();
        var response = await client.GetAsync("/search?searchText=b");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        string content = await response.Content.ReadAsStringAsync();
        var fetchedComics = JsonSerializer.Deserialize<Comic[]>(content);
        Assert.NotNull(fetchedComics);
        Assert.Single(fetchedComics);
        Assert.Single(fetchedComics, c => c.Number == 12);
    }
}