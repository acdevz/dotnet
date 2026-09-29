using ManningBooks.Models;
using Microsoft.EntityFrameworkCore;

namespace ManningBooks.Context;

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
        // if (!dbContext.Database.EnsureCreated())
        // {
        //     return;
        // }
        dbContext.Add(new Book("Grokking Simplicity"));
        dbContext.Add(new Book("API Design Patterns"));
        var efbook = new Book("EF Core in Action");
        efbook.Ratings.Add(new Rating { Comment = "Great!" });
        efbook.Ratings.Add(new Rating { Stars = 4 });
        dbContext.Add(efbook);
        dbContext.Add(new Book("Cloud Native Spring in Action with Microservices"));
        dbContext.SaveChanges();
    }

    public static async Task WriteBookToConsoleAsync(string title)
    {
        using var dbContext = new CatalogContext();
        var book = await dbContext.Books
                    .Include(b => b.Ratings)
                    .FirstOrDefaultAsync(b => b.Title == title);
        if (book == null)
        {
            Console.WriteLine($"\"{title}\" not found.");
            return;
        }
        Console.WriteLine(book);
    }
}