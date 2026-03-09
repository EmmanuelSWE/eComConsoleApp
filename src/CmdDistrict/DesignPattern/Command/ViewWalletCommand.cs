namespace cmdDistrict.DesignPattern.Command;

/// <summary>Wraps the View Wallet Balance action from CustomerMenu.</summary>
public sealed class ViewWalletCommand : ICommand
{
    private readonly Action _execute;
    public string Description => "View your wallet balance";
    public ViewWalletCommand(Action execute) => _execute = execute;
    public void Execute() => _execute();
}
