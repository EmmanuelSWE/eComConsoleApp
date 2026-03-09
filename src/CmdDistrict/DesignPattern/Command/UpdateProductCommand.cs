namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class UpdateProductCommand : ICommand
{
    public string Description => "Update Product — Enter the Product ID then provide a new name, description, price, and stock. Leave a field blank to keep its current value.";

    public void Execute()
    {
        Console.WriteLine("\n  -- Update Product --");
        Console.Write("  Product ID  : "); var id   = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Name        : "); var name = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Description : "); var desc = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Price       : "); var pStr = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Stock       : "); var sStr = Console.ReadLine()?.Trim() ?? "";

        if (!decimal.TryParse(pStr, out var price) || !int.TryParse(sStr, out var stock))
        { Console.WriteLine("  [!] Invalid price or stock."); return; }

        if (Product.Update(GlobalMenuHolder.CurrentUserId!, id, name, desc, price, stock))
            Console.WriteLine("  [✓] Product updated.");
        else
            Console.WriteLine("  [!] Update failed — check product ID and input values.");
    }
}
