namespace cmdDistrict.DesignPattern.Command;

/// <summary>Wraps the Browse Products action from CustomerMenu.</summary>
public sealed class BrowseProductsCommand : ICommand
{
    private readonly Action _execute;
    public string Description => "Browse and search products";
    public BrowseProductsCommand(Action execute) => _execute = execute;
    public void Execute() => _execute();
}
