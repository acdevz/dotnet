using System.ComponentModel.DataAnnotations;

namespace OrderApp.Models.Orders;

public class AddNoteViewModel
{
    public int OrderId { get; set; }

    [Required]
    [StringLength(200)]
    public string Note { get; set; } = string.Empty;
}