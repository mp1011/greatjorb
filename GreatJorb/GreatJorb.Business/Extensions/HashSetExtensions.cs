namespace GreatJorb.Business.Extensions;

public static class HashSetExtensions
{
    public static HashSet<T> Copy<T>(this HashSet<T> h)
    {
        HashSet<T> copied = new();
        foreach (var item in h)
            copied.Add(item);
        return copied;
    }

    public static void AddRange<T>(this HashSet<T> h, IEnumerable<T> items)
    {
        foreach(var item in items)
        {
            h.Add(item);
        }
    }
}
