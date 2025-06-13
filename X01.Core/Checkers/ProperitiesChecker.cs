using System.Reflection;

namespace X01.Core.Checkers;

public class ProperitiesChecker
{
    public static void Check(object obj)
    {
        Type objType = obj.GetType();
        if (objType.IsValueType)
        {
            return;
        }

        PropertyInfo[] objProperties = objType.GetProperties();
        foreach (PropertyInfo? x in objProperties)
        {
            if (typeof(string) == x.PropertyType)
            {
                continue;
            }

            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(x.PropertyType))
            {
                if(x.Name  == "DestinationIds")
                { 
                    continue;
                }
                System.Collections.IEnumerable? enumerable = (System.Collections.IEnumerable?) x.GetValue(obj);
                if(null == enumerable)
                {
                    throw new Exception();
                }
                foreach (object? y in enumerable)
                {
                    Check(y);
                }
            }
        }
    }
}