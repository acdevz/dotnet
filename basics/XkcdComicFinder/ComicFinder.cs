using System.Runtime.InteropServices.Marshalling;

namespace XkcdComicFinder;

public class ComicFinder
{
    private readonly IXkcdClient _xkcdClient;
    private readonly IComicRepository _repo;

    public ComicFinder(IXkcdClient xkcdClient, IComicRepository repo)
    {
        _xkcdClient = xkcdClient;
        _repo = repo;
    }

    public async Task<IAsyncEnumerable<Comic>> FindAsync(string searchText)
    {
        var latestComic = await _xkcdClient.GetLatestAsync();
        int latestInRepo = await _repo.GetLatestNumberAsync();
        if (latestComic.Number != latestInRepo)
        {
            await FetchAsync(latestComic, latestInRepo);
        }
        return _repo.Find(searchText);
    }

    private async Task FetchAsync(Comic latestComic, int latestInRepo)
    {
        await _repo.AddComicAsync(latestComic);
        var tasks = new List<Task<Comic?>>();
        int current = latestComic.Number - 1;
        while (current > latestInRepo)
        {
            tasks.Add(_xkcdClient.GetByNumberAsync(current));
            current--;
        }
        Comic?[] comics = await Task.WhenAll(tasks);
        foreach (var comic in comics.Where(c => c != null))
        {
            await _repo.AddComicAsync(comic!);
        }
    }
}