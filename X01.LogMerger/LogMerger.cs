using X01.Core.Extensions;

namespace X01.LogMerger;

public class LogMerger
{
    private readonly List<string> _ignored_logs = new List<string>(){ 
        "FAPI query errors during last 5 minutes:",
        "invalid_nirvana_trips"
    };
    private readonly LogParser _logParser;

    public LogMerger()
    {
        _logParser = new LogParser();
    }

    public async Task MergeAsync(string[] logFiles, CancellationToken token)
    {
        var logs1 = new List<LogEntry>();
        foreach (var x in logFiles.Skip(1))
        {
            var logs = await MergeAsync(x, token);
            logs1.AddRange(logs);
        }

        await File.WriteAllTextAsync(logFiles[0],
            string.Join(Environment.NewLine + Environment.NewLine,
                logs1.OrderByDescending(x => x.Timestamp)
                    .DistinctBy(x => x.Message)
                    .Where(x=> !_ignored_logs.Any(y=> x.Message.Contains(y)))
                    .DistinctBy(x => string.Join(Environment.NewLine, x.Message.RegexSplitIntoLines()
                                                                       .Skip(1)))
                    .Select(x => x.Timestamp + " [" + x.Tag + "] " + x.Message)),
            token);
    }

    public async Task<IEnumerable<LogEntry>> MergeAsync(string logFile, CancellationToken token)
    {
        var fi = new FileInfo(logFile);
        await using var logStream = fi.OpenRead();
        return await _logParser.ParseAsync(fi.Directory.Name, logStream, token);
    }
}
