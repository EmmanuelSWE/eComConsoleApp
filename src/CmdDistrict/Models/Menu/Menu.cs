namespace cmdDistrict.Models;

/// <summary>
/// Abstract base for all menus.
/// Show() drives the loop while this menu is active.
/// HandleSelection returns true = valid option handled; false = re-prompt.
/// </summary>
public abstract class Menu
{
    public string  Key           { get; protected set; } = "";
    public string  Title         { get; protected set; } = "";
    public string? ContextUserId { get; protected set; }

    public void Show(string? userId)
    {
        ContextUserId = userId;

        while (GlobalMenuHolder.IsCurrentMenu(this))
        {
            Console.WriteLine();
            Console.WriteLine($"=== {Title} ===");
            PrintOptions();
            Console.Write("> ");

            var input = Console.ReadLine()?.Trim() ?? "";

            if (!HandleSelection(input))
                Console.WriteLine("  [!] Invalid selection — please try again.");
        }
    }

    public abstract void PrintOptions();

    /// <returns>true if a valid option was handled; false to re-prompt.</returns>
    public abstract bool HandleSelection(string input);
}
