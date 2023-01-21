namespace ComputopParameterParser.Util
{
    public class StringUtil
    {
        public static string[] Split(string s, int limit = 100)
        {
            var sa = SplitBySymbole(s).Select(x => x.Length > limit ? SplitByDimiliter(x, ' ') : new string[] { x }).SelectMany(x => x).ToArray();
            List<string> result = new List<string>();
            var ss = sa;
            while (0 < ss.Length)
            {
                int cnt = GetCount(ss, limit);
                if(0 == cnt )
                {
                    throw new Exception("bug");
                }
                result.Add(string.Join("", ss.Take(cnt)));
                ss = ss.Skip(cnt).ToArray();
            }
            return result.ToArray();
        }
        private static int GetCount(string[] ss, int limit)
        {
            int sum = 0;
            int i = 0;
            while (i < ss.Length)
            {
                sum += ss[i].Length ;
                if (sum > limit)
                {
                    return i;
                }
                ++i;
            }
            return i;
        }
        private static string[] SplitByDimiliter(string s, char dimiliter)
        {
            var sa = s.Split(dimiliter);
            var sl = sa.Take(sa.Length - 1).Select(x => x + dimiliter).ToList();
            sl.Add(sa.Last());
            return sl.Where(x => !string.IsNullOrEmpty(x)).ToArray();
        }
        private static string[] SplitBySymbole(string s)
        {
            char[] dilimiters = new[] { '.', '!', '?', ',' };
            var sa = SplitByDimiliter(s, '.').Select(x => SplitByDimiliter(x, '!').Select(x => SplitByDimiliter(x, '?')
            .Select(x => SplitByDimiliter(x, '?')).SelectMany(x => x)).SelectMany(x => x)).SelectMany(x => x).ToArray();
            return sa;
        }
    }
}
