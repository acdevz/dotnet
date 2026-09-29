namespace OrderApp.Models.Orders;

public class OrderTrackViewModel
{
    public int OrderId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public DateTimeOffset OrderDate { get; set; }
}