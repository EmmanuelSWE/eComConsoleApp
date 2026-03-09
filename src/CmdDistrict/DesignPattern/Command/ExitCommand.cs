namespace cmdDistrict.DesignPattern.Command;

public sealed class ExitCommand : ICommand
{
    public string Description => "Exit — Closes the application immediately. Make sure you have logged out first if you want your session cleared.";

    public void Execute()
    {
        Console.WriteLine("\n  Goodbye!\n");
        Environment.Exit(0);
    }
}
