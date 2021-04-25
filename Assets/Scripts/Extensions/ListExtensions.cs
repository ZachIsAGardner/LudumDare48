using System.Collections.Generic;
using System.Linq;

public static class ListExtensions
{
    public static void Resize<T>(this List<T> list, int size, T item = default(T))
    {
        int count = list.Count;

        if (size < count)
        {
            list.RemoveRange(size, count - size);
        }
        else if (size > count)
        {
            list.AddRange(Enumerable.Repeat(item, size - count));
        }
    }

    public static List<T> ConcatRange<T>(this List<T> list, List<T> items)
    {
        if (items != null) list.AddRange(items);
        return list;
    }

    public static T Random<T>(this List<T> list)
    {
        int index = Chance.Range(0, list.Count);
        return list[index];
    }
}
