namespace UnitTests.Helpers;

public static class StringExtensions
{
    extension(string str)
    {
        public bool IsGuid()
        {
            return Guid.TryParse(str, out _);
        }
    }
}