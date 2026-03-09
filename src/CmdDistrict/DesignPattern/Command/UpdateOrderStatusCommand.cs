namespace cmdDistrict.DesignPattern.Command;

/// <summary>Wraps the Update Order Status action from AdminMenu.</summary>
public sealed class UpdateOrderStatusCommand : ICommand
{
    private readonly Action _execute;
    public string Description => "Update the status of an order";
    public UpdateOrderStatusCommand(Action execute) => _execute = execute;
    public void Execute() => _execute();
}
