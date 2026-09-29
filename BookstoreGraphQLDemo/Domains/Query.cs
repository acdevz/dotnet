using BookstoreGraphQLDemo.Models;

namespace BookstoreGraphQLDemo.Domains;

public class Query
{
    public Book GetBook() => new Book { Id = 1, Title = "The Correspondent", Genre = "RomCom" };

    public Member GetMember() => new Member { Id = 1, Name = "Martin" };
}

