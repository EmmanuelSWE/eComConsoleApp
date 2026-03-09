namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class ViewCartCommand : ICommand
{
    public string Description => "View Cart — Displays all items currently in your cart with quantities, unit prices, and the running total.";

    public void Execute()
    {
        var userId = GlobalMenuHolder.CurrentUserId!;
        var cart   = Customer.ViewCart(userId);
        if (cart is null || cart.Items.Count == 0)
        { Console.WriteLine("\n  Your cart is empty."); return; }

        Console.WriteLine();
        Console.WriteLine($"  {"Product",-26} {"Unit Price",10}  {"Qty",4}  {"Line Total",11}  {"Product ID"}");
        Console.WriteLine("  " + new string('-', 88));
        foreach (var item in cart.Items)
            Console.WriteLine(
                $"  {item.ProductName,-26} {item.UnitPrice,10:C}  {item.Quantity,4}  {item.UnitPrice * item.Quantity,11:C}  {item.ProductId}");
        Console.WriteLine("  " + new string('-', 88));
        Console.WriteLine($"  {"Total",-42} {Cart.GetTotal(userId),11:C}");
    }
}
