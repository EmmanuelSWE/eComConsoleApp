using cmdDistrict.DesignPattern.Command;

namespace cmdDistrict.Models;

/// <summary>
/// Abstract base for all console menus.
/// Show() drives the render/input/dispatch loop.
/// </summary>
public abstract class Menu
{
    public string  Key           { get; protected set; } = "";
    public string  Title         { get; protected set; } = "";
    public string? ContextUserId { get; private set; }

    /// <summary>All commands registered for this menu, in display order.</summary>
    protected List<ICommand> Commands { get; set; } = new();

    /// <summary>
    /// Runs the menu loop while this menu is the active menu in GlobalMenuHolder.
    /// Sets ContextUserId each iteration so handlers always have fresh context.
    /// </summary>
    public void Show(string? userId)
    {
        while (GlobalMenuHolder.IsCurrentMenu(this))
        {
            ContextUserId = userId ?? GlobalMenuHolder.CurrentUserId;

            Console.WriteLine();
            Console.WriteLine($"  === {Title} ===");
            Console.WriteLine("  " + new string('-', 44));
            PrintOptions();
            Console.WriteLine("  " + new string('-', 44));
            Console.Write("  > ");

            var input = Console.ReadLine()?.Trim() ?? "";

            if (!HandleSelection(input))
                Console.WriteLine("  [!] Invalid option — please try again.");

            // Refresh userId in case role-switch changed it
            userId = GlobalMenuHolder.CurrentUserId;
        }
    }

    /// <summary>Prints the Description of every command in this menu's list.</summary>
    protected void DescribeMenu()
    {
        Console.WriteLine();
        Console.WriteLine($"  === {Title} — Command Descriptions ===");
        Console.WriteLine("  " + new string('-', 44));
        int i = 1;
        foreach (var cmd in Commands)
            Console.WriteLine($"  {i++,2}) {cmd.Description}");
        Console.WriteLine("  " + new string('-', 44));
    }

    /// <summary>Prints the numbered option list for this menu.</summary>
    public abstract void PrintOptions();

    /// <summary>
    /// Dispatches the user's input.
    /// Returns true if input was recognised; false to re-prompt.
    /// </summary>
    public abstract bool HandleSelection(string input);
}
