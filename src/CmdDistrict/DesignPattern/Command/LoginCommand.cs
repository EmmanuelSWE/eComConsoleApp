namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class LoginCommand : ICommand
{
    public string Description => "Login — Enter your registered email and password. On success you are taken to your role's menu (Customer or Admin).";

    public void Execute()
    {
        Console.WriteLine("\n  -- Login --");
        Console.Write("  Email    : "); var email = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Password : "); var pass  = Console.ReadLine()?.Trim() ?? "";

        var (ok, userId, _) = User.Login(email, pass);
        if (ok)
        {
            Console.WriteLine("  [✓] Login successful. Welcome back!");
            GlobalMenuHolder.SwitchToRole(userId);
        }
        else
        {
            Console.WriteLine($"  [!] Login failed: {userId}");
        }
    }
}
