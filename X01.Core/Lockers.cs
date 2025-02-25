using System.Diagnostics;

namespace X01
{
    public sealed class Locker
    {
    }
    public sealed class Lockers
    {
        public static readonly Locker AllocateLargeMemory;
        // public static readonly Locker Cache;
        // public static readonly Locker LargeMemory;
        static Lockers()
        {
            AllocateLargeMemory = new Locker();
        }
    }
    public static class LockerExt
    {
        public static void Run(this Locker locker, Action<CancellationToken?> action, int intervalInMilliseconds = 100, CancellationToken? ct = null)
        {
            // CancellationToken ct = new CancellationToken();
#if DEBUG
            System.Diagnostics.Debug.Assert(null != action);
            Debug.Assert(-1 < intervalInMilliseconds);
#endif
            intervalInMilliseconds = Math.Max(0, intervalInMilliseconds);
            while (true)
            {
                ct?.ThrowIfCancellationRequested();
                if (Monitor.TryEnter(locker))
                {
                    try
                    {
                        action?.Invoke(ct);
                    }
                    finally
                    {
                        Monitor.Exit(locker);
                    }
                    return;
                }
                Thread.Sleep(intervalInMillisec