namespace cmdDistrict.DesignPattern.Command;

/// <summary>Wraps the Low Stock Report action from AdminMenu.</summary>
public sealed class LowStockCommand : ICommand
{
    private readonly Action _execute;
    public string Description => "View products with low stock (< 5)";
    public LowStockCommand(Action execute) => _execute = execute;
    public void Execute() => _execute();
}
