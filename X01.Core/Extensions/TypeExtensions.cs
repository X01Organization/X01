using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace X01.Core.Extensions;

public static class TypeExtensions
{
    /// <summary>
    /// https://www.tutorialsteacher.com/articles/difference-between-string-and-string-in-csharp
    /// </summary>
    private static string? GetTypeAliases(Type type)
    {
        Dictionary<Type, string> aliases = new Dictionary<Type, string>()
        {
            {typeof(int), "int"},
            {typeof(uint), "uint"},
            {typeof(short), "short"},
            {typeof(ushort), "ushort"},
            {typeof(long), "long"},
            {typeof(ulong), "ulong"},
            {typeof(byte), "byte"},
            {typeof(sbyte), "sbyte"},
            {typeof(float), "float"},
            {typeof(double), "double"},
            {typeof(char), "char"},
            {typeof(bool), "bool"},
            {typeof(string), "string"},
            {typeof(decimal), "decimal"},
            {typeof(object), "object"},
        };
        return aliases.TryGetValue(type, out string? alias) ? alias : null;
    }

    private static string GetTypeNameForVariable(Type type)
    {
        if (type.IsArray)
        {
            return type.Name;
        }

        //if (type.IsNullableType())
        if (typeof(Nullable<>).IsAssignableFrom(type))
        {
            return $"{GetTypeNameForVariable(type.GenericTypeArguments[0])}?";
        }

        if (type.IsGenericType)
        {
            return
                //$"{type.Name.Split('`')[0]}<{string.Join(',', type.GenericTypeArguments.Select(GetTypeNameForVariable))}>";
                $"{type.Name.Split('`')[0]}<{string.Join(",", type.GenericTypeArguments.Select(GetTypeNameForVariable))}>";
        }

        return GetTypeAliases(type) ?? type.Name;
    }

    private static object getdefaultvalue(Type t)
    {
        if (t.IsValueType)
        {
            return Activator.CreateInstance(t);
        }

        if (typeof(Nullable<>).IsAssignableFrom(t))
        {
            return getdefaultvalue(t.GetGenericArguments()[0]);
        }

        if (typeof(string) == t)
        {
            return string.Empty;
        }

        object v = Activator.CreateInstance(t);
        SetProperties(v);
        return v;
    }

    private static void SetProperties(object obj)
    {
        Type type = obj.GetType();
        //if (type.IsListType())
        if (typeof(System.Collections.IList).IsAssignableFrom(type))
        {
            System.Collections.IList list = (System.Collections.IList) obj;
            list.Add(getdefaultvalue(type.GetGenericArguments()[0]));
            return;
        }

        if (typeof(System.Collections.IDictionary).IsAssignableFrom(type))
        {
            System.Collections.IDictionary dict = (System.Collections.IDictionary) obj;
            Type[] keyvaluetypes = type.GetGenericArguments();
            Type keytype = keyvaluetypes[0];
            Type valuetype = keyvaluetypes[1];
            if (keytype.IsEnum)
            {
                foreach (object? x in Enum.GetValues(keytype))
                {
                    dict.Add(x, getdefaultvalue(valuetype));
                }

                return;
            }

            object k = getdefaultvalue(type.GetGenericArguments()[0]);
            if (k is string s)
            {
                k = "key";
            }

            object v = getdefaultvalue(type.GetGenericArguments()[1]);
            dict.Add(k, v);
            return;
        }

        PropertyInfo[] properties = type.GetProperties();
        foreach (PropertyInfo? x in properties)
        {
            x.SetValue(obj, getdefaultvalue(x.PropertyType));
        }
    }


    //public static object ConvertValue(this Type type, ) { 
    //    Convert.ChangeType();
    //}
}