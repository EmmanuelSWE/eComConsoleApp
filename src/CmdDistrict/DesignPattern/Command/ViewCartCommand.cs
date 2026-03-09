namespace cmdDistrict.DesignPattern.Command;

/// <summary>Wraps the View Cart action from CustomerMenu.</summary>
public sealed class ViewCartCommand : ICommand
{
    private readonly Action _execute;
    public string Description => "View your current cart";
    public ViewCartCommand(Action execute) => _execute = execute;
    public void Execute() => _execute();
}
