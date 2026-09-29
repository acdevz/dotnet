using Microsoft.AspNetCore.Mvc;
using XkcdComicFinder;

namespace XkcdComicService.Controllers;

[ApiController]
[Route("[controller]")]
public class SearchController : ControllerBase
{
    private readonly ComicFinder _comicFinder;

    public SearchController(ComicFinder comicFinder)
    {
        _comicFinder = comicFinder;
    }

    [HttpGet]
    public Task<IAsyncEnumerable<Comic>> FindAsync(string searchText)
    {
        return _comicFinder.FindAsync(searchText);
    }
}