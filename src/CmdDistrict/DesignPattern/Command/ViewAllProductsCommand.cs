namespace cmdDistrict.DesignPattern.Command;

/// <summary>Wraps the View All Products action from AdminMenu.</summary>
public sealed class ViewAllProductsCommand : ICommand
{
    private readonly Action _execute;
    public string Description => "View all products in the catalog";
    public ViewAllProductsCommand(Action execute) => _execute = execute;
    public void Execute() => _execute();
}
