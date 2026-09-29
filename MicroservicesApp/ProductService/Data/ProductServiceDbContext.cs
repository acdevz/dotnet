using Microsoft.EntityFrameworkCore;
public class ProductServiceDbContext(DbContextOptions<ProductServiceDbContext> options) : DbContext(options)
{
    public DbSet<ProductService.Models.Product> Product { get; set; } = default!;
}
