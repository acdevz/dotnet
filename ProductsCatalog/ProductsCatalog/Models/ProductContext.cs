using Microsoft.EntityFrameworkCore;

namespace ProductsCatalog.Models
{
    public class ProductContext(DbContextOptions<ProductContext> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }

    }
}
