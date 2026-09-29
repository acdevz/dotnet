using ManningBooks.Context;
using Microsoft.Data.Sqlite;

/* *** for Sqlite only *** */
using var keepAliveConnection = new SqliteConnection(CatalogContext.ConnectionString);
keepAliveConnection.Open();
/* *** end *** */

using var dbContext = new CatalogContext();
/* *** sqlite database creation *** */
dbContext.Database.EnsureCreated(); // creates entity model 
/* *** end *** */

CatalogContext.SeedBooks();

/* ***************** eager loading v/s lazy loading ***************** */
var db2Context = new CatalogContext();
var efRatings = db2Context.Books
                .Where(book => book.Title == "EF Core in Action")
                .Select(book => book.Ratings) // eagerly fetching using projections
                .FirstOrDefault();
efRatings?.ForEach(Console.WriteLine);

/* ***************** BONUS 😁 ***************** */
// await dbContext.Books.ForEachAsync(book => Console.WriteLine(book));
// var db2Context = new CatalogContext();
// /* 
//     by default, it goes for lazy loading
//  */
// foreach (var book in db2Context.Books.Include(b => b.Ratings)) // eager loading
// {
//     Console.WriteLine(book);
// }

/* ************************************************************ */
/* ***************** accessing asynchronously ***************** */
/* ************************************************************ */

var userRequests = new[]
{
    ".NET in Action",
    "Grokking Simplicity",
    "API Design Patterns",
    "EF Core in Action"
};
var tasks = userRequests
        .Select(req => CatalogContext.WriteBookToConsoleAsync(req))
        .ToList();
// Task.WaitAll(tasks); // synchronous
await Task.WhenAll(tasks); // asynchronous, equally same behaviour using await!!!