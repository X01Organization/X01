using System.Collections;

namespace X01.Core.Extensions;
public static class StringsExtensions
{
    public static object? ChangeType(this IEnumerable<string?> strings, Type type)
    {
        if (null != Nullable.GetUnderlyingType(type))
        {
            return ChangeType(strings, type.GetGenericArguments().Single());
        }

        if (typeof(IEnumerable).IsAssignableFrom(type) && typeof(string) != type)
        {
        }

        return Convert.ChangeType(strings.SingleOrDefault(), type);
    }

    public static object? ChangeType<TType>(this IEnumerable<string?> strings)
    {
        return strings.ChangeType(typeof(TType));
    }
}
