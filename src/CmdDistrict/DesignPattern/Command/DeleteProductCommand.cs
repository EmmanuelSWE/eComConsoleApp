namespace cmdDistrict.DesignPattern.Command;

/// <summary>Wraps the Delete Product action from AdminMenu.</summary>
public sealed class DeleteProductCommand : ICommand
{
    private readonly Action _execute;
    public string Description => "Delete a product from the catalog";
    public DeleteProductCommand(Action execute) => _execute = execute;
    public void Execute() => _execute();
}
