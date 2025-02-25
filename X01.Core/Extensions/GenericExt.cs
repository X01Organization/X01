namespace X01.Core.Extensions;
public static class GenericExt
{
    public static T? GetDefaultGeneric<T>()
    {
        return default(T);
    }

    public static object? GetDefault(this Type t)
    {
        return typeof(GenericExt).GetMethod(nameof(GetDefaultGeneric) ).MakeGenericMethod(t).Invoke(null, null);
    }
}
