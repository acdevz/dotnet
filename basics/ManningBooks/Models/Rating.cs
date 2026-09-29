namespace ManningBooks.Models;

public class Rating
{
    public int Id { get; set; }
    public int Stars { get; set; } = 5;
    public string? Comment { get; set; }

    public override string ToString()
    {
        return $"Rating {{Id: {Id}, Stars: {Stars}, Comment: {Comment}}}";
    }
}