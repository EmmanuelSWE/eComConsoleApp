namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class AddToCartCommand : ICommand
{
    public string Description => "Add a product to your cart";

    public void Execute()
    {
        Console.WriteLine("\n  -- Add to Cart --");
        Console.Write("  Product ID : "); var productId = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Quantity   : "); var qStr      = Console.ReadLine()?.Trim() ?? "";

        if (!int.TryParse(qStr, out var qty) || qty <= 0)
        { Console.WriteLine("  [!] Quantity must be a positive whole number."); return; }

        if (Cart.AddItem(GlobalMenuHolder.CurrentUserId!, productId, qty))
            Console.WriteLine("  [✓] Item added to cart.");
        else
            Console.WriteLine("  [!] Could not add item — check product ID or available stock.");
    }
}
