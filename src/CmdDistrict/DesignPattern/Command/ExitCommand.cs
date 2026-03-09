namespace cmdDistrict.DesignPattern.Command;

/// <summary>Wraps the Exit action from MainMenu.</summary>
public sealed class ExitCommand : ICommand
{
    private readonly Action _execute;
    public string Description => "Exit the application";
    public ExitCommand(Action execute) => _execute = execute;
    public void Execute() => _execute();
}
