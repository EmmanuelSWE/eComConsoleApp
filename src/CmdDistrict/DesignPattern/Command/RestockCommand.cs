namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class RestockCommand : ICommand
{
    public string Description => "Restock Product — Enter the Product ID and a positive number to increase stock, or a negative number to decrease it. Stock cannot go below zero.";

    public void Execute()
    {
        Console.WriteLine("\n  -- Adjust Inventory --");
        Console.Write("  Product ID          : "); var id   = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Delta (+add/-remove): "); var dStr = Console.ReadLine()?.Trim() ?? "";

        if (!int.TryParse(dStr, out var delta))
        { Console.WriteLine("  [!] Delta must be a whole number (e.g. +10 or -3)."); return; }

        if (Administrator.AdjustInventory(GlobalMenuHolder.CurrentUserId!, id, delta))
            Console.WriteLine("  [✓] Inventory updated.");
        else
            Console.WriteLine("  [!] Failed — check product ID and ensure resulting stock ≥ 0.");
    }
}
