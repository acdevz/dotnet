using System.ComponentModel.DataAnnotations;

namespace OrderService.Entities;

public class OrderStatusEntity
{
    [Key]
    public int StatusId { get; set; }

    public int OrderId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Status { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }

    public bool IsCustomerNote { get; set; }

    public OrderEntity Order { get; set; } = null!;
}