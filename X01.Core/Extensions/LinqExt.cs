namespace X01
{
    public static class LinqExt
    {
        public static IEnumerable<TItem> Yield<TItem>(this TItem item)
        {
            yield return (item);
        }
        public static bool IsNullOrEmpty(System.Collections.IEnumerable enumerable)
        {
            if (null == enumerable)
            {
                return (true);
            }
            foreach (var x in enumerable)
            {
                return (false);
            }
            return (true);
        }
    }
}
