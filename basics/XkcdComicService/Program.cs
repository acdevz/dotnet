using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using XkcdComicFinder;

namespace XkcdComicService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var cfg = builder.Configuration;
        var connStr = cfg.GetConnectionString("Sqlite");
        var baseAddress = cfg.GetValue<string>("BaseAddr");

        var services = builder.Services;
        services.AddScoped<IComicRepository, ComicRepository>();
        services.AddScoped<IXkcdClient, XkcdClient>();
        services.AddScoped<ComicFinder>();

        services.AddControllers();
        services.AddDbContext<ComicDbContext>(options =>
            options.UseSqlite(connStr));
        services.AddHttpClient<IXkcdClient, XkcdClient>(client =>
            client.BaseAddress = new Uri(baseAddress!));

        var app = builder.Build();
        app.MapControllers();

        using var keepAliveConn = new SqliteConnection(connStr);
        keepAliveConn.Open();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider
                .GetRequiredService<ComicDbContext>();
            dbContext.Database.EnsureCreated();
        }

        app.Run();
    }
}