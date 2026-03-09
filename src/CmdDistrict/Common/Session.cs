namespace cmdDistrict.Common;

/// <summary>
/// Holds the current login session state.
/// Set internally by Store methods on successful login/register;
/// cleared on logout.
/// </summary>
public static class Session
{
    public static string? CurrentUserId { get; private set; }
    public static string? CurrentRole   { get; private set; }

    /// <summary>Called by a Store after a successful login or sign-up.</summary>
    public static void Set(string userId, string role)
    {
        CurrentUserId = userId;
        CurrentRole   = role;
    }

    /// <summary>Called by a Store on logout.</summary>
    public static void Clear()
    {
        CurrentUserId = null;
        CurrentRole   = null;
    }
}
