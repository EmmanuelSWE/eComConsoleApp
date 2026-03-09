namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class LogoutCommand : ICommand
{
    public string Description => "Logout — Clears your session and returns you to the Main Menu. Any items left in your cart are saved for your next login.";

    public void Execute()
    {
        User.Logout();
        Console.WriteLine("  [✓] Logged out successfully.");
        GlobalMenuHolder.SignOut();
    }
}
