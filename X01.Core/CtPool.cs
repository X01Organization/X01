namespace X01
{
    public sealed class CtReplacer : ADisposable
    {
        private CancellationTokenSource? cts;

        public CtReplacer()
        {
        }
        public CancellationToken Reset()
        {
            try
            {
                cts?.Cancel();
            }
            catch
            {
            }
            finally
            {
                cts?.Dispose();
            }
            cts = new CancellationTokenSource();
            return (cts.Token);
        }
    }
}
