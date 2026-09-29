using System.ComponentModel.DataAnnotations;

namespace OrderApp.Models.Orders;

public class AddStatusViewModel
{
    public int OrderId { get; set; }

    [Required]
    [StringLength(200)]
    [Display(Name = "New status")]
    public string Status { get; set; } = string.Empty;
}