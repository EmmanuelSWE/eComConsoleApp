namespace cmdDistrict.Common;

/// <summary>Centralized DateTime access — swap out in tests if needed.</summary>
public static class DateProvider
{
    public static DateTime UtcNow => DateTime.UtcNow;
}
