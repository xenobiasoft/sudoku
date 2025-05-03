namespace UnitTests.Helpers;

public static class StringExtensions
{
    public static bool IsGuid(this string str)
    {
        return Guid.TryParse(str, out _);
    }
}