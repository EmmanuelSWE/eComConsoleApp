namespace cmdDistrict.DesignPattern.Command;

public sealed class ExitCommand : ICommand
{
    public string Description => "Exit the application";

    public void Execute()
    {
        Console.WriteLine("\n  Goodbye!\n");
        Environment.Exit(0);
    }
}
