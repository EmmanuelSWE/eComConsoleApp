namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class RemoveCartItemCommand : ICommand
{
    public string Description => "Remove an item from your cart";

    public void Execute()
    {
        Console.Write("\n  Product ID to remove: ");
        var productId = Console.ReadLine()?.Trim() ?? "";

        if (Cart.RemoveItem(GlobalMenuHolder.CurrentUserId!, productId))
            Console.WriteLine("  [✓] Item removed from cart.");
        else
            Console.WriteLine("  [!] Item not found in cart.");
    }
}
