namespace GreatJorb.Business.Extensions;

public static class StringExtensions
{

    public static T TryParseEnum<T>(this string text, T defaultValue)
        where T: struct, Enum
    {
        T result;
        if (Enum.TryParse(text, out result))
            return result;
        else
            return defaultValue;
    }

    public static decimal? TryParseCurrency(this string text)
    {
        text = text.Replace("$", "");
        decimal result;
        if (decimal.TryParse(text, out result))
            return result;
        else
            return null;
    }

    private static string ToLowerAlpha(this string text) => 
        Regex
            .Replace(text, "[^a-zA-Z]", "")
            .ToLower();

    public static T TryParseEnumAdvanced<T>(this string text, T defaultValue)
       where T : struct, Enum
    {
        T result;
        if (Enum.TryParse(text, out result))
            return result;

        text = text.ToLowerAlpha();

        foreach(var enumValue in Enum.GetValues<T>())
        {
            if (enumValue.ToString().ToLowerAlpha() == text)
                return enumValue;
        }

        return defaultValue;
    }


    public static async Task<T> TryParseEnumAdvanced<T>(this Task<string> task)
        where T:struct, Enum
    {
        var text = await task;
        return text.TryParseEnumAdvanced(default(T));
    }
}
