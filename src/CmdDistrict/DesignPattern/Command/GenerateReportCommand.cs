namespace cmdDistrict.DesignPattern.Command;

/// <summary>Wraps the Generate Sales Report action from AdminMenu.</summary>
public sealed class GenerateReportCommand : ICommand
{
    private readonly Action _execute;
    public string Description => "Generate a sales report";
    public GenerateReportCommand(Action execute) => _execute = execute;
    public void Execute() => _execute();
}
