using System.Text.Json;
using System.Text.RegularExpressions;

namespace X01.App.Ja.HttpQueryLogReporter;
public class Reporter
{
    private readonly Option _option;

    public Reporter(Option option)
    {
        _option = option;
    }
#if false
    public async Task Report()
    {
        var logs1 = new List<LogEntry>();
        foreach (var x in logFiles.Skip(1))
        {
            var logs = await MergeAsync(x, token);
            logs1.AddRange(logs);
        }
    }

    public async Task<IEnumerable<LogEntry>> MergeAsync(string logFile, CancellationToken token)
    {
        var fi = new FileInfo(logFile);
        await using var logStream = fi.OpenRead();
        return await _logParser.ParseAsync(fi.Directory.Name, logStream, token);
    }

#endif
    public async Task ReportAsync(CancellationToken token)
    {
        var httpqueryFiles = new DirectoryInfo(_option.InputOrDirectory).GetFiles("httpquery*.log");
        LogParser.LogParser logParser = new LogParser.LogParser();

        foreach (var httpqueryFile in httpqueryFiles)
        {
            var outputFilename = Path.Join(_option.OutputDirectory, httpqueryFile.Name + ".report.json");
            if (File.Exists(outputFilename) && !httpqueryFile.Name.Equals("httpquery.log", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("skipped {0}", httpqueryFile.Name);
                continue;
            }
            Console.WriteLine("reporting {0}", httpqueryFile.Name);

            await using var logStream = httpqueryFile.OpenRead();
            var logEntries = (await logParser.ParseAsync(httpqueryFile.Directory.Name, logStream, token)).ToArray();
            var info = logEntries.Select(x =>
              {
                  string protocal = string.Empty;
                  var m = Regex.Match(x.Message, @"^HTTP\/\d.\d ");
                  if (m.Success)
                  {
                      protocal = m.Groups[0].Value;
                      x.Message = x.Message.Substring(protocal.Length);
                      protocal = protocal.TrimEnd();
                  }

                  string schema = string.Empty;
                  m = Regex.Match(x.Message, @"^https? ");
                  if (m.Success)
                  {
                      schema = m.Groups[0].Value;
                      x.Message = x.Message.Substring(schema.Length);
                      schema = schema.TrimEnd();
                  }

                  var url = string.Empty;
                  var args = string.Empty;
                  int urlEndIndex = x.Message.IndexOf(' ');
                  if (0 > urlEndIndex)
                  {
                      url = x.Message;
                  }
                  else
                  {
                      url = x.Message.Substring(0, urlEndIndex);
                      args = x.Message.Substring(urlEndIndex + 1);
                  }
                  return new
                  {
                      Url = url,
                      QueryParameters = args,
                      Protocol = protocal,
                      Schema = schema,
                      Time = x.Timestamp,
                  };
              })
                .GroupBy(x => x.Url)
                .Select(x => new
                {
                    RequestUrl = x.Key,
                    RequestCount = x.Count(),
                    DistinctRequestCount = x.Select(y => y.QueryParameters).Distinct().Count(),
                    RequestParameters = x
                    .Select(y =>new {y.Time, y.QueryParameters ,})
                    .GroupBy(y=> y.QueryParameters)
                    .Select(y=> new { 
                       QueryParameters = y.Key, 
                       Times = y.Select(z=> z.Time). ToArray(),
                    })
                    .OrderBy(y=> y.Times.Length)
                    .ToArray(),
                })
                .OrderByDescending(x => x.RequestCount)
                .ThenByDescending(x => x.DistinctRequestCount)
                .ToArray();

            var output = JsonSerializer.Serialize(info);

            await File.WriteAllTextAsync(outputFilename, output, token);
        }
    }
}
