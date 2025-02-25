using System.Collections.Generic;
using System.Threading;

namespace X01
{
    public sealed class CtReplacer : ADisposable
    {
        CancellationTokenSource cts;

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
            cts = new CancellationTokenSourc