namespace GreatJorb.Business.Extensions;

public static class StringExtensions
{
    public static bool IsWildcardMatch(this string text, string pattern)
    {
        var parts = pattern.Split('*', StringSplitOptions.RemoveEmptyEntries);
         
        var regex = string.Join(")(.*)(", parts);

        regex = $"({regex})";

        if (pattern.StartsWith("*"))
            regex = $"(.*){regex}";

        if (pattern.EndsWith("*"))
            regex = $"{regex}(.*)";

        return Regex.IsMatch(text, regex, RegexOptions.IgnoreCase);
    }

    public static string UrlEncode(this string? text)
    {
        if (text == null)
            return string.Empty;

        return HttpUtility.UrlEncode(text);
    }

    public static string HtmlEncode(this string? text)
    {
        if (text == null)
            return string.Empty;

        return HttpUtility.HtmlEncode(text);
    }

    public static int? TryParseIntOrDefault(this string? text)
    {
        if (text == null)
            return null;

        int result;
        if (int.TryParse(text, out result))
            return result;
        else
            return null;
    }

    public static int TryParseInt(this string? text, int defaultReturn)
    {
        if (text == null)
            return defaultReturn;

        int result;
        if (int.TryParse(text, out result))
            return result;
        else
            return defaultReturn;
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

    public static decimal? TryParseCurrency(this string text)
    {
        decimal multiply = 1.0m;

        if(text.EndsWith("K", StringComparison.OrdinalIgnoreCase))
        {
            text = text.Substring(0, text.Length - 1);
            multiply = 1000.0m;
        }
        text = text.Replace("$", "");
        decimal result;
        if (decimal.TryParse(text, out result))
            return result * multiply;
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

    public static string ToShortMoney(this decimal? d)
    {
        if (d == null)
            return string.Empty;

        d = d / 1000;
        return $"${d.Value.ToString("0.0")}k";
    }

    public static string ResolvePathVariables(this string s)
    {
        return s.Replace("%AppData%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
    }

    public static bool IsNullOrEmpty(this string s) => String.IsNullOrEmpty(s);
}
