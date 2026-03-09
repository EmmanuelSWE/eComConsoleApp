namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.DataAccess;
using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class ViewWalletCommand : ICommand
{
    public string Description => "View Wallet — Displays your current wallet balance. Use Deposit Funds to top up before placing an order.";

    public void Execute()
    {
        using var ctx = new AppDbContext();
        var customer = ctx.Users.OfType<Customer>().FirstOrDefault(u => u.Id == GlobalMenuHolder.CurrentUserId);
        if (customer is not null)
            Console.WriteLine($"\n  Wallet Balance: {customer.WalletBalance:C}");
        else
            Console.WriteLine("  [!] Could not retrieve wallet.");
    }
}
