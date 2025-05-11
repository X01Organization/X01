// See https://aka.ms/new-console-template for more information
using X01.LogMerger;

CancellationTokenSource tokenSource = new();

await new LogMerger().MergeAsync(args,tokenSource.Token);
