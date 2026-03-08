using cmdDistrict.Models.Entities;

namespace cmdDistrict.Models;

/// <summary>
/// Static router: registers all menus, tracks CurrentUserId / CurrentRole,
/// and routes to Customer or Admin menu after login/register.
/// </summary>
public static class GlobalMenuHolder
{
    private static readonly Dictionary<string, Menu> _menus = new();
    private static Menu _current = null!;

    public static string? CurrentUserId { get; private set; }
    public static string? CurrentRole   { get; private set; }

    /// <summary>Called by Menu.Show() each iteration to detect a menu switch.</summary>
    public static bool IsCurrentMenu(Menu menu) => ReferenceEquals(_current, menu);

    public static void Register(Menu menu) => _menus[menu.Key] = menu;

    public static void Switch(string key)
    {
        if (_menus.TryGetValue(key, out var m))
            _current = m;
        else
        {
            Console.WriteLine($"  [Error] Menu '{key}' not found — returning to main.");
            _current = _menus["main"];
        }
    }

    /// <summary>
    /// Sets CurrentUserId, resolves role via User.GetRole, and routes to the
    /// appropriate role menu.
    /// </summary>
    public static void SwitchToRole(string userId)
    {
        try
        {
            CurrentUserId = userId;
            CurrentRole   = User.GetRole(userId);
            Switch(CurrentRole == "Administrator" ? "admin" : "customer");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"  [Error] {ex.Message}");
            CurrentUserId = null;
            CurrentRole   = null;
            Switch("main");
        }
    }

    /// <summary>Clears user context and returns to main menu (used on logout).</summary>
    public static void SignOut()
    {
        CurrentUserId = null;
        CurrentRole   = null;
        Switch("main");
    }

    /// <summary>Registers Main / Customer / Admin menus; sets starting menu to Main.</summary>
    public static void Start()
    {
        Register(new MainMenu());
        Register(new CustomerMenu());
        Register(new AdminMenu());
        Switch("main");
    }

    /// <summary>Main event loop.</summary>
    public static void Run()
    {
        while (true)
            _current.Show(CurrentUserId);
    }
}
