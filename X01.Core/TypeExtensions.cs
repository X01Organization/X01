using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.Json.Serialization;
using System.IO;

namespace Journaway.Core.Xiang;

public class TypeExtensions
{
    /// <summary>
    /// https://www.tutorialsteacher.com/articles/difference-between-string-and-string-in-csharp
    /// </summary>
    private string GetTypeAliases(Type type)
    {
        var aliases = new Dictionary<Type, string>()
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
        return aliases.TryGetValue(type, out var alias) ? alias : null;
    }

    private string GetTypeNameForVariable(Type type)
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

    private void test111()
    {
        var tripType = typeof(TypeExtensions);
        var contentTypeNamespace = tripType.Namespace;

        var contentTypeTypes = tripType.Assembly
                                       .GetTypes()
                                       .Where(x => x.Namespace == contentTypeNamespace &&
                                                   x.IsClass &&
                                                   !x.IsAbstract)
                                       .ToArray();
        foreach (var contentTypeType in contentTypeTypes)
        {
            var contentTypeProperties = contentTypeType.GetProperties()
                                                       .Where(x => JsonIgnoreCondition.Always
                                                                != x.GetCustomAttribute<JsonIgnoreAttribute>()
                                                                   ?.Condition)
                                                       .ToArray();
            StringBuilder sb = new StringBuilder();
            sb.Append("using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine();
            sb.Append("namespace API.Contentful.Contents;");
            sb.AppendLine();
            sb.Append($"public class {contentTypeType.Name}");
            sb.AppendLine();
            sb.Append("{");
            sb.AppendLine();
            foreach (var contentTypeProperty in contentTypeProperties)
            {
                var type = contentTypeProperty.PropertyType.GenericTypeArguments[1];
                var name = contentTypeProperty.Name;
                var typeName = GetTypeNameForVariable(type);
                if (typeName == "Link")
                {
                    typeName = name;
                }

                if (typeName == "List<Link>")
                {
                    typeName = $"List<{name}>";
                }

                //if (!typeName.EndsWith('?'))
                if (!typeName.EndsWith("?"))
                {
                    typeName = typeName + '?';
                }

                string propertydefination = $"public {typeName} {name} " + "{get; set;}";
                sb.Append($"    {propertydefination}");
                sb.AppendLine();
            }

            sb.Append("}");

            File.WriteAllText($"c:/workroot/tmp/{contentTypeType.Name}.cs", sb.ToString());
        }
    }

    private object getdefaultvalue(Type t)
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

        var v = Activator.CreateInstance(t);
        SetProperties(v);
        return v;
    }

    private void SetProperties(object obj)
    {
        var type = obj.GetType();
        //if (type.IsListType())
        if (typeof(System.Collections.IList).IsAssignableFrom(type))
        {
            var list = (System.Collections.IList) obj;
            list.Add(getdefaultvalue(type.GetGenericArguments()[0]));
            return;
        }

        if (typeof(System.Collections.IDictionary).IsAssignableFrom(type))
        {
            var dict = (System.Collections.IDictionary) obj;
            var keyvaluetypes = type.GetGenericArguments();
            var keytype = keyvaluetypes[0];
            var valuetype = keyvaluetypes[1];
            if (keytype.IsEnum)
            {
                foreach (var x in Enum.GetValues(keytype))
                {
                    dict.Add(x, getdefaultvalue(valuetype));
                }

                return;
            }

            var k = getdefaultvalue(type.GetGenericArguments()[0]);
            if (k is string s)
            {
                k = "key";
            }

            var v = getdefaultvalue(type.GetGenericArguments()[1]);
            dict.Add(k, v);
            return;
        }

        var properties = type.GetProperties();
        foreach (var x in properties)
        {
            x.SetValue(obj, getdefaultvalue(x.PropertyType));
        }
    }
}