namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class UpdateCartQtyCommand : ICommand
{
    public string Description => "Update Cart Quantity — Enter the Product ID of a cart item and the new quantity. Enter 0 to remove the item. Stock is adjusted accordingly.";

    public void Execute()
    {
        Console.WriteLine("\n  -- Update Cart Item Quantity --");
        Console.Write("  Product ID   : "); var productId = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  New Quantity : "); var qStr      = Console.ReadLine()?.Trim() ?? "";

        if (!int.TryParse(qStr, out var newQty) || newQty <= 0)
        { Console.WriteLine("  [!] Quantity must be a positive whole number."); return; }

        if (CartItem.UpdateQuantity(GlobalMenuHolder.CurrentUserId!, productId, newQty))
            Console.WriteLine("  [✓] Cart updated.");
        else
            Console.WriteLine("  [!] Update failed — check product ID or stock availability.");
    }
}
