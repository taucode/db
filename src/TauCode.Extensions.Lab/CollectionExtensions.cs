namespace TauCode.Extensions;

public static class CollectionExtensions
{
    public static void AddRangeToCollection<T>(this IEnumerable<T> target, IEnumerable<T> source)
    {
        switch (target)
        {
            case List<T> list:
                list.AddRange(source);
                return;

            case ICollection<T> collection:
                {
                    foreach (var v in source)
                    {
                        collection.Add(v);
                    }

                    break;
                }
        }

        throw new ArgumentException("Collection does not support adding.", nameof(target));
    }
}