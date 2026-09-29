using System.Net;
using ManningBooksApiSecure.Context;
using ManningBooksApiSecure.Controllers;
using ManningBooksApiSecure.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace ManningBooksApiSecure.Tests;

public class CatalogControllerTests : IDisposable
{
    private const string ConnStr = "DataSource=test.db;cache=shared;mode=memory";
    private readonly CatalogContext _catalogContext;
    private readonly SqliteConnection _keepAliveConn;
    private readonly CatalogController _controller;

    public CatalogControllerTests(ITestOutputHelper testOutput)
    {
        _keepAliveConn = new SqliteConnection(ConnStr);
        _keepAliveConn.Open();

        var options = new DbContextOptionsBuilder();
        options.UseSqlite(ConnStr);
        options.AddInterceptors(new LogSqlInterceptor(testOutput));

        /* EF Logging directly without using any interceptor */
        options.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
        options.EnableSensitiveDataLogging();
        /* ************************************************* */

        _catalogContext = new CatalogContext(options.Options);
        _controller = new CatalogController(_catalogContext);

        Assert.True(_catalogContext.Database.EnsureCreated());
    }

    [Fact]
    public async Task GetBook()
    {
        SeedBooks();
        var book = await _controller.GetBook(1);
        Assert.NotNull(book);
    }

    private void SeedBooks()
    {
        _catalogContext.Add(new Book("Grokking Machine Learning"));
        _catalogContext.Add(new Book("Grokking System Design"));
        var efCore = new Book("EF Core in Action");
        efCore.Ratings.Add(new Rating() { Comment = "Great!" });
        efCore.Ratings.Add(new Rating() { Stars = 3 });
        _catalogContext.Add(efCore);
        _catalogContext.SaveChanges();
    }

    public void Dispose()
    {
        _keepAliveConn.Close();
        _catalogContext.Dispose();
    }
}