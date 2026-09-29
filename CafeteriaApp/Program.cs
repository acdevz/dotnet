using CafeteriaApp.Models;
using CafeteriaApp.Services;

var manager = new OrderManager();
int choice;

do
{
    Console.WriteLine("\n=== COMPANY CAFETERIA SYSTEM ===");
    Console.WriteLine("1. Show Menu");
    Console.WriteLine("2. Place Order");
    Console.WriteLine("3. View All Orders");
    Console.WriteLine("4. Search Order");
    Console.WriteLine("5. Cancel Order");
    Console.WriteLine("0. Exit");
    Console.Write("Enter your choice: ");

    if (!int.TryParse(Console.ReadLine(), out choice)) continue;

    switch (choice)
    {
        case 1:
            Console.WriteLine("\n--- CAFETERIA MENU ---");
            foreach (var menu in manager.ShowMenu()) Console.WriteLine(menu);
            break;

        case 2:
            Console.Write("Enter Employee Name: ");
            string? name = Console.ReadLine();
            Console.WriteLine("\n--- CAFETERIA MENU ---");
            foreach (var menu in manager.ShowMenu()) Console.WriteLine(menu);
            var itemIds = new List<int>();

            while (true)
            {
                Console.Write("Enter Item ID to add (0 to finish): ");
                if (!int.TryParse(Console.ReadLine(), out int id) || id < 0)
                {
                    Console.WriteLine("Invalid input");
                    continue;
                }
                else if (id != 0) itemIds.Add(id);
                if (id == 0)
                {
                    int? ordId = manager.PlaceOrder(itemIds, name ?? "Unknown");
                    if (ordId == null)
                    {
                        Console.WriteLine("Item not found.");
                        break;
                    }
                    else Console.WriteLine($"Order placed: {manager.SearchOrder(ordId ?? 0)}");
                    break;
                }
            }
            break;

        case 3:
            foreach (var ord in manager.GetOrders()) Console.WriteLine(ord);
            break;

        case 4:
            Console.Write("Enter Order ID: ");
            if (!int.TryParse(Console.ReadLine(), out int orderId))
            {
                Console.WriteLine("Invalid Order ID.");
                break;
            }
            var result = manager.SearchOrder(orderId);
            if (result != null) Console.WriteLine(result);
            else Console.WriteLine("Order not found");
            break;

        case 5:
            Console.Write("Enter Order ID to cancel: ");
            if (!int.TryParse(Console.ReadLine(), out int cancellableOrderId))
            {
                Console.WriteLine("Invalid Order ID.");
                break;
            }
            if (manager.CancelOrder(cancellableOrderId)) Console.WriteLine("Order cancelled");
            else Console.WriteLine("Order not found");
            break;

        case 0:
            Console.WriteLine("Goodbye!");
            break;

        default:
            Console.WriteLine("Invalid choice.");
            break;
    }
}
while (choice != 0);
