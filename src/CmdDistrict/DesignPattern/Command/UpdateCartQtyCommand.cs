namespace cmdDistrict.DesignPattern.Command;

/// <summary>Wraps the Update Cart Item Quantity action from CustomerMenu.</summary>
public sealed class UpdateCartQtyCommand : ICommand
{
    private readonly Action _execute;
    public string Description => "Update a cart item's quantity";
    public UpdateCartQtyCommand(Action execute) => _execute = execute;
    public void Execute() => _execute();
}
