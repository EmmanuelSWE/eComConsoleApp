namespace cmdDistrict.DesignPattern.Command;

/// <summary>Wraps the Add to Cart action from CustomerMenu.</summary>
public sealed class AddToCartCommand : ICommand
{
    private readonly Action _execute;
    public string Description => "Add a product to your cart";
    public AddToCartCommand(Action execute) => _execute = execute;
    public void Execute() => _execute();
}
