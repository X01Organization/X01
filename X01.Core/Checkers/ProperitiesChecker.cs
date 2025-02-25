namespace X01.Core.Checkers;

public class ProperitiesChecker
{
    public static void Check(object obj)
    {
        var objType = obj.GetType();
        if (objType.IsValueType)
        {
            return;
        }

        var objProperties = objType.GetProperties();
        foreach (var x in objProperties)
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
                var enumerable = (System.Collections.IEnumerable) x.GetValue(obj);
                if(null == enumerable)
                {
                    throw new Exception();
                }
                foreach (var y in enumerable)
                {
                    Check(y);
                }
            }
        }
    }
}