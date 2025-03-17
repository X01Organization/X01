using System.Runtime.CompilerServices;

namespace X01.Core.Extensions;

public static class AsyncEnumerableExtension
{
    public static object InvokeAsyncEnumerableMethod(object asyncEnumerable, Type methodGenericType, [CallerMemberName] string methodName = "")
    {
        System.Reflection.MethodInfo enumerableMethod = typeof(AsyncEnumerable).GetMethod(methodName);
        if (null == enumerableMethod)
        {
            throw new ArgumentException($"{nameof(methodName)}({methodName}) not found");
        }

        return enumerableMethod!.MakeGenericMethod(methodGenericType).Invoke(null, new[] { asyncEnumerable, });
    }

    public static object Cast( object asyncEnumerable, Type castType)
    {
        return InvokeAsyncEnumerableMethod(asyncEnumerable!, castType);
    }

    public static object ToList( object asyncEnumerable, Type castType)
    {
        return InvokeAsyncEnumerableMethod(asyncEnumerable!, castType);
    }
}
