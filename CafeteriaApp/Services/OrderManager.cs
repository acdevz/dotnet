using System.Net.Http.Headers;
using CafeteriaApp.Models;

namespace CafeteriaApp.Services;

public class OrderManager
{
    private Dictionary<int, MenuItem> menu = new Dictionary<int, MenuItem>();
    private List<Order> orders = new List<Order>();
    private int nextOrderId = 1;

    public OrderManager()
    {
        menu.Add(1, new MenuItem(1, "Coffee", 2.5));
        menu.Add(2, new MenuItem(2, "Sandwich", 4.0));
        menu.Add(3, new MenuItem(3, "Pasta", 6.0));
        menu.Add(4, new MenuItem(4, "Juice", 3.0));
        menu.Add(5, new MenuItem(5, "Salad", 3.5));
    }

    public List<MenuItem> ShowMenu()
    {
        List<MenuItem> menuItems = new List<MenuItem>();
        foreach (KeyValuePair<int, MenuItem> pair in menu)
        {
            menuItems.Add(pair.Value);
        }
        return menuItems;
    }

    public int? PlaceOrder(List<int> itemIds, string name)
    {
        try
        {
            var orderId = nextOrderId++;
            var order = new Order(orderId, name);
            foreach (int itemId in itemIds)
                order.Items.Add(menu[itemId]);
            order.CalculateTotal();
            orders.Add(order);
            return orderId;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public Order? SearchOrder(int id)
    {
        return orders.Find(order => order.OrderId == id);
    }

    public bool CancelOrder(int id)
    {
        Order? cancelledOrder = SearchOrder(id);
        return cancelledOrder == null ? false : orders.Remove(cancelledOrder);
    }

    public void AddOrderForTest(string name, List<int> itemIds)
    {
        if (PlaceOrder(itemIds, name) == null)
            throw new Exception("Test failed");
    }

    public List<Order> GetOrders()
    {
        return orders;
    }
}