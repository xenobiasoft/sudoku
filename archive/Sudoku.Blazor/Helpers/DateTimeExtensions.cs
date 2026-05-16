namespace Sudoku.Blazor.Helpers;

public static class DateTimeExtensions
{
    public static string ToLocalDateTime(this DateTime dateTime)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.Local).ToString("g");
    }
}