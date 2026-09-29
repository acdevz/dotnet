namespace BookstoreGraphQLDemo.Models;

public class Book
{
    public int Id { get; set; }      // Unique identifier
    public string? Title { get; set; } // Book title
    public string? Genre { get; set; } // Category/genre
}
