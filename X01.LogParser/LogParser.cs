
using System.Text.RegularExpressions;
using X01.Core.Extensions;

namespace X01.LogParser;

public class LogParser
{
    public async Task<IEnumerable<LogEntry> > ParseAsync(string tag, Stream logStream, CancellationToken token)
    {
        var log = await logStream.ReadToEndAsync();
                string pattern = @"(?<timestamp>\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{4}) \[(?<threadId>\d+)\] (?<logType>\w+) \[(?<logLevel>\w+)\] \[(?<classNamespace>[\w.]+)\] - (?<message>[\s\S]*?)(?=\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{4} \[|\z)";
                Regex regex = new Regex(pattern);
                var matches = regex.Matches(log);
  // Process each match
  return matches.Select(match=> new LogEntry
            {
Tag = tag,
                Timestamp = match.Groups["timestamp"].Value,
                ThreadId = match.Groups["threadId"].Value,
                LogType = match.Groups["logType"].Value,
                LogLevel = match.Groups["logLevel"].Value,
                ClassNamespace = match.Groups["classNamespace"].Value,
                Message = match.Groups["message"].Value.Trim()
            });
    }
    #if false
    private DateTime? GetLogTime(string? logLine)
    {
        if(string.IsNullOrWhiteSpace(logLine))
        {
            return null;
        }

                string pattern = @"(?<logTime>\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{4}) \[(?<threadId>\d+)\] (?<logLevel>\w+) \[(?<logType>\w+)\] \[(?<class>[\w.]+)\] - (?<logMessage>.*)";

                Regex regex = new Regex(pattern);
                Match match = regex.Match(logLine);

        var logTimeLength = 24;
        var logTimeEg = "2023-11-28 09:28:16.9716 [1] ERROR [verbose] [DotnetRestSvc.Startup] - "; 
        if (!(logTimeEg.Length <= logLine?.Length))
        {
            return null;
        }
        var possibleLogTime = logLine!.Substring(0, logTimeEg.Length);
        if ( !DateTime.TryParse(possibleLogTime, out var logTime))
        {
            return null;
        }
        return null;
    }
    #endif
}
public class LogEntry
{
    public string? Tag { get; set; }
    public string? Timestamp { get; set; }
    public string? ThreadId { get; set; }
    public string? LogType { get; set; }
    public string? LogLevel { get; set; }
    public string? ClassNamespace { get; set; }
    public string? Message { get; set; }
}
