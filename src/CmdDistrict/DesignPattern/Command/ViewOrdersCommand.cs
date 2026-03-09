namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class ViewOrdersCommand : ICommand
{
    public string Description => "View your order history";

    public void Execute()
    {
        var orders = Customer.ViewOrders(GlobalMenuHolder.CurrentUserId!);
        if (orders.Count == 0) { Console.WriteLine("\n  No orders found."); return; }

        Console.WriteLine();
        Console.WriteLine($"  {"Date",-20} {"Status",-12} {"Total",9}  {"Order ID"}");
        Console.WriteLine("  " + new string('-', 80));
        foreach (var o in orders)
            Console.WriteLine(
                $"  {o.CreatedAt:yyyy-MM-dd HH:mm,-20} {o.Status,-12} {o.Total,9:C}  {o.Id}");
    }
}
