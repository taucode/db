namespace TauCode.Extensions;

public static class CollectionExtensionsLab
{
    public static void AddMany<T>(this IEnumerable<T> target, IEnumerable<T> source)
    {
        if (target == null!)
        {
            throw new ArgumentNullException(nameof(target));
        }

        if (source == null!)
        {
            throw new ArgumentNullException(nameof(source));
        }

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