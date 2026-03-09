namespace cmdDistrict.DesignPattern.Command;

/// <summary>Wraps the Register action from MainMenu.</summary>
public sealed class RegisterCommand : ICommand
{
    private readonly Action _execute;
    public string Description => "Register a new user account";
    public RegisterCommand(Action execute) => _execute = execute;
    public void Execute() => _execute();
}
