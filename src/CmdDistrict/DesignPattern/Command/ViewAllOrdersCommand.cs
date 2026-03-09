namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class ViewAllOrdersCommand : ICommand
{
    public string Description => "View All Orders — Lists every order across all customers, newest first, with Order ID, customer, status, total, and date.";

    public void Execute()
    {
        var orders = Administrator.ListAllOrders(GlobalMenuHolder.CurrentUserId!);
        if (orders.Count == 0) { Console.WriteLine("\n  No orders found."); return; }

        Console.WriteLine();
        Console.WriteLine($"  {"Date",-20} {"Customer ID",-36} {"Status",-12} {"Total",9}  {"Order ID"}");
        Console.WriteLine("  " + new string('-', 105));
        foreach (var o in orders)
            Console.WriteLine(
                $"  {o.CreatedAt:yyyy-MM-dd HH:mm,-20} {o.CustomerId,-36} {o.Status,-12} {o.Total,9:C}  {o.Id}");
    }
}
