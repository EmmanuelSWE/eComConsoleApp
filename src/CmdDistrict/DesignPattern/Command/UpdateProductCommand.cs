namespace cmdDistrict.DesignPattern.Command;

/// <summary>Wraps the Update Product action from AdminMenu.</summary>
public sealed class UpdateProductCommand : ICommand
{
    private readonly Action _execute;
    public string Description => "Update an existing product";
    public UpdateProductCommand(Action execute) => _execute = execute;
    public void Execute() => _execute();
}
