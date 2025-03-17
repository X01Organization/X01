using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using X01.Core.Extensions;

namespace X01.CmdLine;

public class CmdLineArgsParser
{
    public T Parse<T>(string[] args) where T : class
    {
        Console.WriteLine("paring args:{0}{1}", Environment.NewLine, string.Join(Environment.NewLine, args));
        IEnumerable<CmdLineArg> cmdLineArgs = Parse(args);
        return Convert<T>(cmdLineArgs);
    }

    private IEnumerable<CmdLineArg> Parse(string[] args)
    {
        CmdLineArg? lastLongOrShortArg = null;
        //https://regex101.com/r/FADDVO/1
        Regex shortArgRegex = new Regex(@"^(\-{1})([a-zA-Z0-9]+)(=(.+))?$");
        Regex longArgRegex = new Regex(@"^(\-{2})([a-zA-Z0-9_\-]+)(=(.+))?$");
        foreach (string x in args)
        {
            Match longArgMatch = longArgRegex.Match(x);
            if (longArgMatch.Success)
            {
                string name = longArgMatch.Groups[2].Value;
                string value = longArgMatch.Groups[4].Value;
                CmdLineLongArg arg = new CmdLineLongArg(name, value);
                lastLongOrShortArg = arg;
                yield return arg;
            }
            else
            {
                Match shortArgMatch = shortArgRegex.Match(x);
                if (shortArgMatch.Success)
                {
                    string name = shortArgMatch.Groups[2].Value;
                    string value = shortArgMatch.Groups[4].Value;
                    CmdLineShortArgs arg = new CmdLineShortArgs(name, value);
                    lastLongOrShortArg = arg;
                    yield return arg;
                }
                else
                {
                    if (null != lastLongOrShortArg && string.IsNullOrEmpty(lastLongOrShortArg!.Value))
                    {
                        lastLongOrShortArg!.Value = x;
                    }
                    else
                    {
                        yield return new CmdLineValueArg(x);
                    }
                }
            }
        }
    }

    private T Convert<T>(IEnumerable<CmdLineArg> cmdLineArgs) where T : class
    {
        cmdLineArgs = cmdLineArgs.ToArray()
            .SelectMany(
           x => x is CmdLineShortArgs cmdLineShortArgs
           ? cmdLineShortArgs.Args.Cast<CmdLineArg>()
           : new[] { x });

        ILookup<string, CmdLineShortArg> cmdLineShortArgsByName = cmdLineArgs
            .Where(x => x is CmdLineShortArg)
            .Cast<CmdLineShortArg>()
            .ToLookup(x => x.Name);

        ILookup<string, CmdLineLongArg> cmdLineLongArgsByName = cmdLineArgs
            .Where(x => x is CmdLineLongArg)
            .Cast<CmdLineLongArg>()
            .ToLookup(x => x.Name);

        IEnumerable<string?> values = cmdLineArgs
            .Where(x => x is not CmdLineLongArg && x is not CmdLineLongArg)
            .Select(x => x.Value);

        PropertyInfo[] properties = typeof(T).GetProperties();
        T t = Activator.CreateInstance<T>();

        foreach (PropertyInfo x in properties)
        {
            foreach (CmdLineArgsAttribute y in x.GetCustomAttributes(typeof(CmdLineArgsAttribute), true).Cast<CmdLineArgsAttribute>())
            {
                List<string?> propertyValues = new();
                if (!string.IsNullOrWhiteSpace(y.ShortName) &&
                    cmdLineShortArgsByName.Contains(y.ShortName!))
                {
                    propertyValues.AddRange(cmdLineShortArgsByName[y.ShortName!].Select(z => z.Value));
                }

                if (!string.IsNullOrWhiteSpace(y.LongName) &&
                    cmdLineLongArgsByName.Contains(y.LongName!))
                {
                    propertyValues.AddRange(cmdLineLongArgsByName[y.LongName!].Select(z => z.Value));
                }

                SetPropertyValues(t, x, propertyValues);
            }
        }

        return t;
    }

    private void SetPropertyValues<T>(T t, PropertyInfo propertyInfo, List<string?> propertyValues)
    {
        object? value = ChangeType(propertyValues, propertyInfo.PropertyType);
        propertyInfo.SetValue(t, value);
    }

    public static object? ChangeType(IEnumerable<string?> strings, Type type)
    {
        if (null != Nullable.GetUnderlyingType(type))
        {
            bool noValue = strings.Any(x=> !string.IsNullOrEmpty(x));
            if(noValue)
            { 
                return type.GetDefault();
            }

            return ChangeType(strings, type.GetGenericArguments().Single());
        }

        if (typeof(IEnumerable).IsAssignableFrom(type) && typeof(string) != type)
        {
            Type genericType = type.GetGenericArguments().Single();
            return strings.Select(x => ChangeType(x, genericType))
                .Cast(genericType)
                .ToList(genericType);
        }

        string? s = strings.SingleOrDefault();

        return ChangeType(s, type);
    }

    private static object? ChangeType(string? s , Type type)
    { 
        if(string.IsNullOrEmpty(s))
        { 
            return type.GetDefault();
        }

        Console.WriteLine("converting ({0}) to {1}", s, type.Name);
        return System.Convert.ChangeType(s, type);
    }

    public static object? ChangeType<TType>(IEnumerable<string?> strings)
    {
        return ChangeType(strings, typeof(TType));
    }

}
