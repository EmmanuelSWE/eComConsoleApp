namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class DepositCommand : ICommand
{
    public string Description => "Deposit funds into your wallet";

    public void Execute()
    {
        Console.Write("\n  Deposit amount: ");
        var aStr = Console.ReadLine()?.Trim() ?? "";

        if (!decimal.TryParse(aStr, out var amount) || amount <= 0)
        { Console.WriteLine("  [!] Amount must be a positive number."); return; }

        if (Customer.Deposit(GlobalMenuHolder.CurrentUserId!, amount))
            Console.WriteLine($"  [✓] Deposited {amount:C} to your wallet.");
        else
            Console.WriteLine("  [!] Deposit failed.");
    }
}
