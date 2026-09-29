namespace ManningBooksApiSecure.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public List<Rating> Ratings { get; } = []; // -- new List<>() --> new() --> [] 🤯

    public Book(string title, string? description = null)
    {
        Title = title;
        Description = description;
    }

    public override string ToString()
    {
        var ratingList = string.Join(", ", Ratings);
        return $"Book {{Id: {Id}, Title: {Title}, Ratings: [{ratingList}]}}";
    }
}