namespace cmdDistrict.DesignPattern.Command;

/// <summary>Wraps the Add Product Review action from CustomerMenu.</summary>
public sealed class AddReviewCommand : ICommand
{
    private readonly Action _execute;
    public string Description => "Submit a product review";
    public AddReviewCommand(Action execute) => _execute = execute;
    public void Execute() => _execute();
}
