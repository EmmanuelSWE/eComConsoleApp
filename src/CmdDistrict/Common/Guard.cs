namespace cmdDistrict.Common;

public static class Guard
{
    public static void NotNull(object? value, string name)
    {
        if (value is null)
            throw new ArgumentNullException(name, $"{name} must not be null.");
    }

    public static void NotEmpty(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{name} must not be empty or whitespace.", name);
    }
}
