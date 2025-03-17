using System.Runtime.CompilerServices;

namespace X01.Core.Extensions;
public static class EnumerableExtensions
{
    private static object InvokeEnumerableMethod(object enumerable, Type methodGenericType, [CallerMemberName] string methodName = "")
    {
        System.Reflection.MethodInfo enumerableMethod = typeof(Enumerable).GetMethod(methodName);
        if (null == enumerableMethod)
        {
            throw new ArgumentException($"{nameof(methodName)}({methodName}) not found");
        }

        return enumerableMethod!.MakeGenericMethod(methodGenericType)
            .Invoke(null, new[] { enumerable, });
    }

    public static object Cast(this object enumerable, Type castType)
    {
        return InvokeEnumerableMethod(enumerable!, castType);
    }

    public static object ToList(this object enumerable, Type castType)
    {
        return InvokeEnumerableMethod(enumerable!, castType);
    }
}
