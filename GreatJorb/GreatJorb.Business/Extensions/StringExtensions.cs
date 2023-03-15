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

    public static string SubstringUpTo(this string? text, char endExclusive)
    {
        if (text!.IsNullOrEmpty())
            return string.Empty;

        var index = text!.IndexOf(endExclusive);
        if (index < 0)
            return text ?? string.Empty;

        return text.Substring(0, index);
    }

    public static string SubstringFrom(this string? text, char start)
    {
        if (text!.IsNullOrEmpty())
            return string.Empty;

        var index = text!.IndexOf(start);
        if (index < 0)
            return text ?? string.Empty;

        return text.Substring(index);
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
        if(int.TryParse(text, out _))
            return defaultValue; 

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
        int workHoursPerYear = 2080;

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

    public static string ChangeRelativePath(this string url, string relativePath)
    {
        var uri = new Uri(url);
        return uri.GetLeftPart(UriPartial.Authority) + relativePath;
    }

    public static string? GetQuerystringOrHashValue(this string url, string key)
    {
        var query = url.SubstringFrom('?');
        if (query.IsNullOrEmpty())
            return null;

        var queryWithoutHash = query.SubstringUpTo('#');
        var parsedQuery = HttpUtility.ParseQueryString(queryWithoutHash);

        var result = parsedQuery[key];
        if (result != null)
            return result.Split(',').Last();

        var hashQuery = query.SubstringFrom('#');
        parsedQuery = HttpUtility.ParseQueryString(hashQuery);
        return parsedQuery[key];
    }
}
