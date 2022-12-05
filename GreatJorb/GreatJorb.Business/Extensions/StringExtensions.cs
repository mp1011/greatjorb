namespace GreatJorb.Business.Extensions;

public static class StringExtensions
{
    public static WorkplaceType ParseWorkplaceType(this string s)
    {
        s = Regex.Replace(s, "[^a-zA-Z]", "");

        return s.TryParseEnum(WorkplaceType.Unknown);
    }

    public static T TryParseEnum<T>(this string text, T defaultValue)
        where T: struct, Enum
    {
        T result;
        if (Enum.TryParse(text, out result))
            return result;
        else
            return defaultValue;
    }


    public static async Task<WorkplaceType> ParseWorkplaceType(this Task<string> task)
    {
        var text = await task;
        return text.ParseWorkplaceType();
    }
}
