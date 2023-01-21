namespace createsql
{
    public class Class1
    {
        public IEnumerable<string> Split(long count, int chunk, string sql )
        {
            string condition = @" AND ""Uid"" BETWEEN ";
            long c = count / chunk;
            long m = count % chunk;
            int i = 1;
            while (i <= c)
            {
                int start = i == 1 ? 0 : (((i - 1) * chunk) + 1);
                int end = i * chunk;
                ++i;
                yield return sql + condition + start + " AND " + end + ";";
            }

            if (m > 0)
            {
                yield return sql + " AND \"Uid\" > " + (c * chunk) + ";";
            }
        }
    }
}