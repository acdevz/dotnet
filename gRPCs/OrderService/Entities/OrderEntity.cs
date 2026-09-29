using System.ComponentModel.DataAnnotations;

namespace OrderService.Entities;

public class OrderEntity
{
    [Key]
    public int OrderId { get; set; }

    [Required]
    [MaxLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; }

    public ICollection<OrderStatusEntity> Statuses { get; set; } = new List<OrderStatusEntity>();
}