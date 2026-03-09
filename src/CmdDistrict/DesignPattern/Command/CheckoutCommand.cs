namespace cmdDistrict.DesignPattern.Command;

/// <summary>Wraps the Checkout action from CustomerMenu.</summary>
public sealed class CheckoutCommand : ICommand
{
    private readonly Action _execute;
    public string Description => "Checkout and process payment";
    public CheckoutCommand(Action execute) => _execute = execute;
    public void Execute() => _execute();
}
