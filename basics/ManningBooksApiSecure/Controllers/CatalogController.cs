using System.Runtime.InteropServices;
using ManningBooksApiSecure.Context;
using ManningBooksApiSecure.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ManningBooksApiSecure.Controllers;

[ApiController]
[Route("catalog")]
public class CatalogController : ControllerBase
{
    private readonly CatalogContext _dbContext;
    public CatalogController(CatalogContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public IAsyncEnumerable<Book> GetBooks(string? title)
    {
        IQueryable<Book> query = _dbContext.Books
            .Include(b => b.Ratings)
            .AsNoTracking();
        if (title != null)
        {
            query = query.Where(b => b.Title.ToLower().Contains(title.ToLower()));
        }
        return query.AsAsyncEnumerable();
    }

    [HttpGet("{id}")]
    public Task<Book?> GetBook(int id)
    {
        return _dbContext.Books.FirstOrDefaultAsync(
            b => b.Id == id
        );
    }

    [HttpPost]
    public async Task<Book> CreateBookAsync(BookCreateCommand command, CancellationToken token)
    {
        var book = new Book(command.Title, command.Description);
        var entity = _dbContext.Books.Add(book);
        await _dbContext.SaveChangesAsync(token);
        return entity.Entity;
    }

    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateBookAsync(
        int id, BookUpdateCommand command, CancellationToken token
    )
    {
        var book = await _dbContext.FindAsync<Book>([id], token);
        if (book == null) return NotFound();
        if (command.Title != null) book.Title = command.Title;
        if (command.Description != null) book.Description = command.Description;

        await _dbContext.SaveChangesAsync(token);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteBookAsync(
        int id, CancellationToken token
    )
    {
        var book = await _dbContext.Books.Include(b => b.Ratings).FirstOrDefaultAsync(b => b.Id == id, token);
        if (book == null) return NotFound();
        _dbContext.Remove(book);
        await _dbContext.SaveChangesAsync(token);
        return NoContent();
    }


    /* ************** DTOs ************** */

    public record BookCreateCommand(string Title, string? Description) { };
    public record BookUpdateCommand(string? Title, string? Description) { };
}