namespace cmdDistrict.DesignPattern.Command;

/// <summary>Wraps the guest Browse Products action from MainMenu.</summary>
public sealed class BrowseGuestCommand : ICommand
{
    private readonly Action _execute;
    public string Description => "Browse products as a guest";
    public BrowseGuestCommand(Action execute) => _execute = execute;
    public void Execute() => _execute();
}
