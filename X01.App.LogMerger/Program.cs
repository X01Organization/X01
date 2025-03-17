// See https://aka.ms/new-console-template for more information
using X01.LogMerger;

CancellationTokenSource tokenSource = new CancellationTokenSource();

await new LogMerger().MergeAsync(args,tokenSource.Token);
