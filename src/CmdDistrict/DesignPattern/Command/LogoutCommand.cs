namespace cmdDistrict.DesignPattern.Command;

/// <summary>Wraps the Logout action from CustomerMenu and AdminMenu.</summary>
public sealed class LogoutCommand : ICommand
{
    private readonly Action _execute;
    public string Description => "Log out of your account";
    public LogoutCommand(Action execute) => _execute = execute;
    public void Execute() => _execute();
}
