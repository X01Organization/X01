// See https://aka.ms/new-console-template for more information
using X01.LogMerger;

var tokenSource = new CancellationTokenSource();

await new LogMerger().MergeAsync(args,tokenSource.Token);
