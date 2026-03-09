namespace cmdDistrict.DesignPattern.Command;

/// <summary>Wraps the Add Product action from AdminMenu.</summary>
public sealed class AddProductCommand : ICommand
{
    private readonly Action _execute;
    public string Description => "Add a new product to the catalog";
    public AddProductCommand(Action execute) => _execute = execute;
    public void Execute() => _execute();
}
