namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class GenerateReportCommand : ICommand
{
    public string Description => "Generate a sales report";

    public void Execute()
    {
        Console.WriteLine("\n  -- Generate Sales Report --");
        Console.Write("  From (yyyy-MM-dd): "); var fromStr = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  To   (yyyy-MM-dd): "); var toStr   = Console.ReadLine()?.Trim() ?? "";

        if (!DateTime.TryParse(fromStr, out var from) || !DateTime.TryParse(toStr, out var to))
        { Console.WriteLine("  [!] Invalid date format — use yyyy-MM-dd."); return; }

        var report = Administrator.GenerateReport(GlobalMenuHolder.CurrentUserId!, from, to.AddDays(1).AddSeconds(-1));
        Console.WriteLine();
        Console.WriteLine(report);
    }
}
