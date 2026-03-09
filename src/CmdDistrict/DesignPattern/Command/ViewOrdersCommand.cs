namespace cmdDistrict.DesignPattern.Command;

/// <summary>Wraps the View My Orders action from CustomerMenu.</summary>
public sealed class ViewOrdersCommand : ICommand
{
    private readonly Action _execute;
    public string Description => "View your order history";
    public ViewOrdersCommand(Action execute) => _execute = execute;
    public void Execute() => _execute();
}
