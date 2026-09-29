using Microsoft.EntityFrameworkCore;

namespace XkcdComicFinder;

public class ComicDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Comic> Comics { get; set; } = null!; // null fogiving operator - something wont be null though
}