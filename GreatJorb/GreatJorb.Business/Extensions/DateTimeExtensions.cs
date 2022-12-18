namespace GreatJorb.Business.Extensions;

public static class DateTimeExtensions
{
    public static TimeSpan TimeSince(this DateTime d)
    {
        return DateTime.Now - d;
    }
}
