namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class RemoveCartItemCommand : ICommand
{
    public string Description => "Remove Cart Item — Enter the Product ID of the item you want to remove. The reserved stock is returned to the product.";

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
