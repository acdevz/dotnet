using ManningBooksApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ManningBooksApi.Context;

public class CatalogContext : DbContext
{
    // const -- implicitly static
    public const string ConnectionString = "DataSource=manningbooks;cache=shared"; // mode=memory
    public DbSet<Book> Books { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(ConnectionString);
    }

    public static void SeedBooks()
    {
        using var dbContext = new CatalogContext();
        if (!dbContext.Database.EnsureCreated())
        {
            return;
        }
        dbContext.Add(new Book("Grokking Simplicity"));
        dbContext.Add(new Book("API Design Patterns"));
        var efbook = new Book("EF Core in Action");
        efbook.Ratings.Add(new Rating { Comment = "Great!" });
        efbook.Ratings.Add(new Rating { Stars = 4 });
        dbContext.Add(efbook);
        dbContext.Add(new Book("Cloud Native Spring in Action with Microservices"));
        dbContext.SaveChanges();
    }
}