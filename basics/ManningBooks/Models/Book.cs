namespace ManningBooks.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public List<Rating> Ratings { get; } = []; // -- new List<>() --> new() --> [] 🤯

    public Book(string title)
    {
        Title = title;
    }

    public override string ToString()
    {
        var ratingList = string.Join(", ", Ratings);
        return $"Book {{Id: {Id}, Title: {Title}, Ratings: [{ratingList}]}}";
    }
}