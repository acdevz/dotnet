using BookstoreGraphQLDemo.Models;

namespace BookstoreGraphQLDemo.Domains;

public class Mutation
{
    public Book AddBook(string title, string genre)
    {
        return new Book
        {
            Id = 2,
            Title = title,
            Genre = genre
        };
    }

    public Member AddMember(string name)
    {
        return new Member
        {
            Id = 2,
            Name = name
        };
    }
}
