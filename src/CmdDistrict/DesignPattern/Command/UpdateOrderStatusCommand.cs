namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class UpdateOrderStatusCommand : ICommand
{
    public string Description => "Update Order Status — Enter an Order ID and choose the new status (Paid, Shipped, Delivered, Cancelled). Only valid status transitions are allowed.";

    public void Execute()
    {
        Console.WriteLine("\n  -- Update Order Status --");
        Console.Write("  Order ID                                              : "); var orderId   = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  New status (Pending/Paid/Packed/Shipped/Delivered/Cancelled): ");
        var statusStr = Console.ReadLine()?.Trim() ?? "";

        if (!Enum.TryParse<OrderStatus>(statusStr, ignoreCase: true, out var status))
        { Console.WriteLine("  [!] Invalid status. Options: Pending, Paid, Packed, Shipped, Delivered, Cancelled."); return; }

        if (Order.UpdateStatus(GlobalMenuHolder.CurrentUserId!, orderId, status))
            Console.WriteLine($"  [✓] Order status updated to {status}.");
        else
            Console.WriteLine("  [!] Failed — invalid transition or order not found.");
    }
}
