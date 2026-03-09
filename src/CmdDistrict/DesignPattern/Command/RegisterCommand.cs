namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.DataAccess;
using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class RegisterCommand : ICommand
{
    public string Description => "Register — Enter your name, email, password, and role (Customer/Administrator). A new account will be created and you will be signed in automatically.";

    public void Execute()
    {
        Console.WriteLine("\n  -- Register --");
        Console.Write("  Name                                    : "); var name      = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Email                                   : "); var email     = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Password                                : "); var pass      = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Role (1 = Customer, 2 = Administrator)  : "); var roleInput = Console.ReadLine()?.Trim() ?? "";

        var role    = roleInput == "2" ? "Administrator" : "Customer";
        var address = "";

        if (role == "Customer")
        {
            Console.Write("  Default Shipping Address                : "); address = Console.ReadLine()?.Trim() ?? "";
        }

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
        {
            Console.WriteLine("  [!] Name, email, and password must not be empty.");
            return;
        }
        if (role == "Customer" && string.IsNullOrEmpty(address))
        {
            Console.WriteLine("  [!] Shipping address must not be empty for Customer accounts.");
            return;
        }

        var (ok, userId) = User.Sign(name, email, pass, role);
        if (ok)
        {
            if (role == "Customer" && !string.IsNullOrEmpty(address))
            {
                using var ctx = new AppDbContext();
                var customer = ctx.Users.OfType<Customer>().SingleOrDefault(u => u.Id == userId);
                if (customer is not null)
                {
                    customer.DefaultShippingAddress = address;
                    ctx.SaveChanges();
                }
            }
            Console.WriteLine($"  [✓] Registered successfully. Welcome, {name}!");
            GlobalMenuHolder.SwitchToRole(userId);
        }
        else
        {
            Console.WriteLine("  [!] Registration failed — email may already be in use.");
        }
    }
}
