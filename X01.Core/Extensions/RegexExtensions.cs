using System.Text.RegularExpressions;

namespace X01.Core.Extensions;

public static class RegexExtensions
{
    public static string[] RegexSplitIntoLines(this string s)
    {
        return Regex.Split(s, "\r\n|\r|\n");
    }
}
