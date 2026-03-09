namespace cmdDistrict.DesignPattern.Command;

/// <summary>Wraps the Login action from MainMenu.</summary>
public sealed class LoginCommand : ICommand
{
    private readonly Action _execute;
    public string Description => "Log in to your account";
    public LoginCommand(Action execute) => _execute = execute;
    public void Execute() => _execute();
}
