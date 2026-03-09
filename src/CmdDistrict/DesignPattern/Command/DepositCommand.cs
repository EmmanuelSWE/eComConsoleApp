namespace cmdDistrict.DesignPattern.Command;

/// <summary>Wraps the Deposit Funds action from CustomerMenu.</summary>
public sealed class DepositCommand : ICommand
{
    private readonly Action _execute;
    public string Description => "Deposit funds into your wallet";
    public DepositCommand(Action execute) => _execute = execute;
    public void Execute() => _execute();
}
