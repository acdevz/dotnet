using ManningBooksApiSecure.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var ConnStr = config.GetConnectionString("Catalog");
builder.Services.AddDbContext<CatalogContext>(options =>
    options.UseSqlite(ConnStr));

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = DateTime.UtcNow.AddYears(1) - DateTime.UtcNow;
});

var app = builder.Build();
app.UseHttpsRedirection();
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    using var dbContext = scope.ServiceProvider.GetRequiredService<CatalogContext>();
    dbContext.Database.EnsureCreated();
    CatalogContext.SeedBooks(dbContext);
}

app.MapGet("/", () => "Health Status: Ok 🟢");

app.Run();
