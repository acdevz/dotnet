using Microsoft.EntityFrameworkCore;

public class OrderServiceDbContext(DbContextOptions<OrderServiceDbContext> options) : DbContext(options)
{
    public DbSet<Order> Order { get; set; } = default!;
}
