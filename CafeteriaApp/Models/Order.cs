namespace CafeteriaApp.Models;

public class Order
{
    public int OrderId { get; set; }
    public string? EmployeeName { get; set; }
    public List<MenuItem> Items { get; set; } = new List<MenuItem>();
    public double Total { get; set; }

    public DateTime Date { get; set; }

    public Order(int id, string employeeName)
    {
        OrderId = id;
        EmployeeName = employeeName;
        Date = DateTime.Now;
    }

    public void CalculateTotal()
    {
        Total = Items.Sum(i => i.Price);
    }

    public override string ToString()
    {
        string itemList = string.Join(", ", Items.Select(i => i.Name));
        return $"Order #{OrderId} | {EmployeeName} | Items: {itemList} | Total: ${Total:F2}";
    }
}