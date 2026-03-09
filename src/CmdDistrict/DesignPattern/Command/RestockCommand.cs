namespace cmdDistrict.DesignPattern.Command;

/// <summary>Wraps the Restock Product action from AdminMenu.</summary>
public sealed class RestockCommand : ICommand
{
    private readonly Action _execute;
    public string Description => "Adjust product inventory";
    public RestockCommand(Action execute) => _execute = execute;
    public void Execute() => _execute();
}
