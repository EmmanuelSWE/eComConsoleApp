namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class LogoutCommand : ICommand
{
    public string Description => "Log out of your account";

    public void Execute()
    {
        User.Logout();
        Console.WriteLine("  [✓] Logged out successfully.");
        GlobalMenuHolder.SignOut();
    }
}
