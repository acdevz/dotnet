using System.ComponentModel.DataAnnotations;

namespace OrderApp.Models.Orders;

public class CreateOrderViewModel
{
    [Required]
    [StringLength(100)]
    [Display(Name = "Customer name")]
    public string CustomerName { get; set; } = string.Empty;
}