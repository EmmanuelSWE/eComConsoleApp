namespace cmdDistrict.DesignPattern.Command;

/// <summary>Wraps the Remove Item from Cart action from CustomerMenu.</summary>
public sealed class RemoveCartItemCommand : ICommand
{
    private readonly Action _execute;
    public string Description => "Remove an item from your cart";
    public RemoveCartItemCommand(Action execute) => _execute = execute;
    public void Execute() => _execute();
}
