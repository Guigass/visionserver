namespace VisionAPI.Tools;

public static class Date
{
    public static long ToUnixEpochDate(DateTime date) => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

    public static bool IsBewteenTwoDates(this DateTime dt, DateTime start, DateTime end)
    {
        return dt >= start && dt <= end;
    }
}
