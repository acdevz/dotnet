using Microsoft.EntityFrameworkCore;

namespace XkcdComicFinder;

public class ComicRepository : IComicRepository
{
    private readonly ComicDbContext _dbContext;

    public ComicRepository(ComicDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddComicAsync(Comic comic)
    {
        _dbContext.Add(comic);
        return _dbContext.SaveChangesAsync();
    }

    public IAsyncEnumerable<Comic> Find(string searchText)
    {
        return _dbContext.Comics.Where(
            c => c.Title != null && c.Title.Contains(searchText) ||
                c.Alt != null && c.Alt.Contains(searchText))
            .AsAsyncEnumerable();
    }

    public Task<int> GetLatestNumberAsync()
    {
        return _dbContext.Comics.OrderByDescending(c => c.Number)
            .Select(c => c.Number)
            .FirstOrDefaultAsync();
    }
}