namespace cmdDistrict.Common;

/// <summary>Argument-validation helpers.</summary>
public static class Guard
{
    public static void NotNull(object? value, string paramName)
    {
        if (value is null)
            throw new ArgumentNullException(paramName, $"'{paramName}' must not be null.");
    }

    public static void NotEmpty(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"'{paramName}' must not be empty or whitespace.", paramName);
    }
}
