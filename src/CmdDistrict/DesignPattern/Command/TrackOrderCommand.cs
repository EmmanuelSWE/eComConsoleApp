namespace cmdDistrict.DesignPattern.Command;

/// <summary>Wraps the Track Order action from CustomerMenu.</summary>
public sealed class TrackOrderCommand : ICommand
{
    private readonly Action _execute;
    public string Description => "Track the status of an order";
    public TrackOrderCommand(Action execute) => _execute = execute;
    public void Execute() => _execute();
}
