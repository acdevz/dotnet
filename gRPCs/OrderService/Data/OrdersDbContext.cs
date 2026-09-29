using Microsoft.EntityFrameworkCore;
using OrderService.Entities;

namespace OrderService.Data;

public class OrdersDbContext : DbContext
{
    public OrdersDbContext(
        DbContextOptions<OrdersDbContext> options)
        : base(options)
    {
    }

    public DbSet<OrderEntity> Orders => Set<OrderEntity>();

    public DbSet<OrderStatusEntity> OrderStatuses =>
        Set<OrderStatusEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OrderEntity>(entity =>
        {
            entity.ToTable("Orders");

            entity.HasKey(x => x.OrderId);

            entity.Property(x => x.OrderId)
                .ValueGeneratedOnAdd();

            entity.Property(x => x.CustomerName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.OrderDate)
                .IsRequired();
        });

        modelBuilder.Entity<OrderStatusEntity>(entity =>
        {
            entity.ToTable("OrderStatuses");

            entity.HasKey(x => x.StatusId);

            entity.Property(x => x.Status)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Timestamp)
                .IsRequired();

            entity.Property(x => x.IsCustomerNote)
                .IsRequired();

            entity.HasOne(x => x.Order)
                .WithMany(x => x.Statuses)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}