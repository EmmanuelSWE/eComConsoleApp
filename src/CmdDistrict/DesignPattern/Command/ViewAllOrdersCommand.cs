namespace cmdDistrict.DesignPattern.Command;

/// <summary>Wraps the View All Orders action from AdminMenu.</summary>
public sealed class ViewAllOrdersCommand : ICommand
{
    private readonly Action _execute;
    public string Description => "View all customer orders";
    public ViewAllOrdersCommand(Action execute) => _execute = execute;
    public void Execute() => _execute();
}
