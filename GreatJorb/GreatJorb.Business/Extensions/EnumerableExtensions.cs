namespace GreatJorb.Business.Extensions;

public static class EnumerableExtensions
{
    public static void SplitByCondition<T>(this IEnumerable<T> source, Predicate<T> condition, ref T[] match, ref T[] noMatch)
    {
        List<T> matchList = new();
        List<T> noMatchList = new();

        foreach(var item in source)
        {
            if(condition(item))
                matchList.Add(item);
            else
                noMatchList.Add(item);
        }

        match = matchList.ToArray();
        noMatch = noMatchList.ToArray();
    }

}
