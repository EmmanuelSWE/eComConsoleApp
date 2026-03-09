namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class TrackOrderCommand : ICommand
{
    public string Description => "Track Order — Enter an Order ID to see its current status (Pending, Paid, Shipped, Delivered, or Cancelled).";

    public void Execute()
    {
        Console.Write("\n  Order ID: ");
        var orderId = Console.ReadLine()?.Trim() ?? "";
        try
        {
            var status = Order.TrackStatus(GlobalMenuHolder.CurrentUserId!, orderId);
            Console.WriteLine($"  Order {orderId} is currently: {status}");
        }
        catch
        {
            Console.WriteLine("  [!] Order not found.");
        }
    }
}
