using System.Buffers.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Playwright;

namespace PlaywrightXunit.Tests;

public class ApiTests : IAsyncLifetime
{
    private IPlaywright? _playwright; // playwright engine
    private IAPIRequestContext? _context; // to trigger api endpoint
    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _context = await _playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions()
        {
            BaseURL = "http://localhost:5085",
            IgnoreHTTPSErrors = true,
            ExtraHTTPHeaders = new Dictionary<string, string> { { "Accept", "application/json" } }
        });
    }

    [Fact]
    public async Task GetBook_Catalog()
    {
        Assert.NotNull(_context);

        var res = await _context.GetAsync("/catalog/3");
        Assert.NotNull(res);
        Assert.Equal(200, res.Status);

        var body = await res.JsonAsync();
        Assert.NotNull(body);

        /* APPROACH-1 */

        var root = body.Value;

        Assert.Equal(3, root.GetProperty("id").GetInt32());
        Assert.Equal("EF Core in Action", root.GetProperty("title").GetString());

        Assert.Equal(JsonValueKind.Null, root.GetProperty("description").ValueKind);

        var ratings = root.GetProperty("ratings");

        Assert.Equal(JsonValueKind.Array, ratings.ValueKind);
        Assert.Equal(2, ratings.GetArrayLength());

        var rating1 = ratings[0];
        Assert.Equal(1, rating1.GetProperty("id").GetInt32());
        Assert.Equal(5, rating1.GetProperty("stars").GetInt32());
        Assert.Equal("Great!", rating1.GetProperty("comment").GetString());

        var rating2 = ratings[1];
        Assert.Equal(2, rating2.GetProperty("id").GetInt32());
        Assert.Equal(4, rating2.GetProperty("stars").GetInt32());
        Assert.Equal(JsonValueKind.Null, rating2.GetProperty("comment").ValueKind);

        /* APPROACH-2 */
        // var book = await res.JsonAsync<BookDto>();

        // Assert.NotNull(book);

        // Assert.Equal(3, book.Id);
        // Assert.Equal("EF Core in Action", book.Title);
        // Assert.Null(book.Description);

        // Assert.Equal(2, book.Ratings.Count);

        // Assert.Equal(1, book.Ratings[0].Id);
        // Assert.Equal(5, book.Ratings[0].Stars);
        // Assert.Equal("Great!", book.Ratings[0].Comment);

        // Assert.Equal(2, book.Ratings[1].Id);
        // Assert.Equal(4, book.Ratings[1].Stars);
        // Assert.Null(book.Ratings[1].Comment);
    }

    [Fact]
    public async Task GetAllBooks_Catalog()
    {
        Assert.NotNull(_context);
        var res = await _context.GetAsync("/catalog");

        Assert.NotNull(res);
        Assert.Equal(200, res.Status);

        var body = await res.JsonAsync();
        Assert.NotNull(body);

        var root = body.Value;
        Assert.Equal(JsonValueKind.Array, root.ValueKind);

        var books = root.EnumerateArray();
        Assert.NotEmpty(books);
    }

    [Fact]
    public async Task PostBook_Catalog()
    {
        Assert.NotNull(_context);
        var res = await _context.PostAsync("/catalog", new APIRequestContextOptions()
        {
            DataObject = new Dictionary<string, string>
            {
                {"title", "Learn Kuberenetes in a Month of Lunches"},
                {"description", "Kubernetes with extended lunchbreaks by Elton Stoneman"}
            }
        });

        Assert.NotNull(res);
        Assert.Equal(200, res.Status);

        var body = await res.JsonAsync();
        Assert.NotNull(body);

        var root = body.Value;
        Assert.Equal(
            "Learn Kuberenetes in a Month of Lunches",
            root.GetProperty("title").GetString());
        Assert.Equal(
            "Kubernetes with extended lunchbreaks by Elton Stoneman",
            root.GetProperty("description").GetString());
    }

    [Fact]
    public async Task DeleteBook_Catalog()
    {
        Assert.NotNull(_context);
        var res = await _context.PostAsync("/catalog", new APIRequestContextOptions()
        {
            DataObject = new Dictionary<string, string>
            {
                {"title", "ToBeDeleted"},
            }
        });

        Assert.NotNull(res);
        Assert.Equal(200, res.Status);

        var body = await res.JsonAsync();
        Assert.NotNull(body);
        int id = body.Value.GetProperty("id").GetInt32();

        res = await _context.DeleteAsync($"/catalog/{id}");
        Assert.NotNull(res);
        Assert.Equal(204, res.Status);
    }

    public async Task DisposeAsync()
    {
        _playwright?.Dispose();
        if (_context != null) await _context.DisposeAsync();
    }
}

public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public List<RatingDto> Ratings { get; set; } = [];
}

public class RatingDto
{
    public int Id { get; set; }
    public int Stars { get; set; }
    public string? Comment { get; set; }
}