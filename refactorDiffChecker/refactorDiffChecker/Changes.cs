namespace refactorDiffChecker
{
    public class Changes
    {
        public string Removed { get; }
        public string Added { get; }

        public Changes(string removed, string added)
        {
            Removed = removed;
            Added = added;
        }

        public override int GetHashCode()
        {
            return new
            {
                Removed,
                Added,
            }.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is Changes c)
            {
                return c.Removed == Removed && c.Added == Added;
            }

            return false;
        }

        public bool NoChange => Removed == Added;
    }
}