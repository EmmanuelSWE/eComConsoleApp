namespace cmdDistrict.DesignPattern.Command;

/// <summary>Contract for all menu commands in Cmd District.</summary>
public interface ICommand
{
    /// <summary>Human-readable description of what this command does.</summary>
    string Description { get; }

    /// <summary>Executes the command.</summary>
    void Execute();
}
