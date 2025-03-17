using X01.Core.Extensions;
using X01.LogParser;

namespace X01.LogMerger;

public class LogMerger
{
    private readonly List<string> _ignored_logs = new List<string>(){ 
        "FAPI query errors during last 5 minutes:",
        "invalid_nirvana_trips",
        "Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException: The database operation was expected to affect",
        "System.Threading.Tasks.TaskCanceledException",
        "System.OperationCanceledException: The operation was canceled.\n",
        "System.ObjectDisposedException: Cannot access a disposed object.\nObject name: 'IServiceProvider'.",
        "Couldn't create receiver. API-Response: StatusCode: 400",
        " ---> Npgsql.NpgsqlException (0x80004005): Exception while reading from stream\n ---> System.TimeoutException: Timeout during reading attempt",
        "System.InvalidOperationException: An exception has been raised that is likely due to a transient failure.\n ---> System.TimeoutException:",
        " ---> Npgsql.NpgsqlException (0x80004005): Failed to connect to ",
        "Unable to connect to any of the specified MySQL hosts.",
        "Path: GET:www.journaway.com/api/BookingRoute/GetBookingStepVisibilities",
        " ---> System.Net.Sockets.SocketException (111): Connection refused",
        "---> System.IO.IOException:  Received an unexpected EOF or 0 bytes from the transport stream.",
        "BadHttpRequestException: Unexpected end of request content.",
        "API.HubSpot.NET.Core.HubSpotException: Error from HubSpot, JSONResponse=Empty",
        "MySqlConnector.MySqlException (0x80004005): The Command Timeout expired before the operation completed.",
        "---> MySqlConnector.MySqlException (0x80004005): Connect Timeout expired.",
        "Missing translationKey=airport.",
        "---> Npgsql.NpgsqlException (0x80004005): The operation has timed out",
        "---> Npgsql.PostgresException (0x80004005): 57P01: terminating connection due to administrator command",
        "---> StackExchange.Redis.RedisConnectionException: It was not possible to connect to the redis server(s). ConnectTimeout",
        "StackExchange.Redis.RedisConnectionException: The message timed out in the backlog attempting to send because no connection became available",
        "System.OutOfMemoryException: Exception of type 'System.OutOfMemoryException' was thrown.",
        " was invalid:",
        "RateHawk cancellation failed for BookingCostPositionUid ",
        "Failed executing DbCommand",
        "System.IO.IOException: Unable to read data from the transport connection: Operation canceled.",
        "System.IO.IOException: Unable to read data from the transport connection: Connection reset by peer",
        "Npgsql.PostgresException (0x80004005): 57014: canceling statement due to user request",
        "Npgsql.PostgresException (0x80004005): 53300: remaining connection slots are reserved for",
        "Npgsql.PostgresException (0x80004005): 53300: sorry, too many clients already",
        "Npgsql.PostgresException (0x80004005): 23505: duplicate key value violates unique constraint",
        "Npgsql.NpgsqlException (0x80004005): Failed to connect to",
        "Npgsql.PostgresException (0x80004005): 57P03: the database system is shutting down",
        "Npgsql.PostgresException (0x80004005): 57P03: the database system is starting up",
        "(0x80004005): The connection pool has been exhausted, either raise",
        "Could not process spookie, the scary tracking cookie. Parsed content:"",
        ").DayDescription must be set",
        "Missing translationKey=contentful.accommodation.room.Whirlpool",
        ".dayDescription must have content because",
        ") not found in contentful",
        "Hangfire.PostgreSql.PostgreSqlDistributedLockException",
        "SixLabors.ImageSharp.Formats.ImageFormatManager.ThrowInvalidDecoder",
    };
    private readonly LogParser.LogParser _logParser;

    public LogMerger()
    {
        _logParser = new LogParser.LogParser();
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
            string.Join(Environment.NewLine,
                logs1.OrderByDescending(x => x.Timestamp)
                    .DistinctBy(x => x.Message)
                    .Where(x=> x.Message!.Contains("Application startup exception") || !_ignored_logs.Any(y=> x.Message.Replace("\r\n", "\n").Contains(y)))
                    .Select(x=> new { Error = x, MsgLines = x.Message!.RegexSplitIntoLines().Where(y=> !string.IsNullOrWhiteSpace(y))})
                    .GroupBy(x=> x.MsgLines.Skip(1).Any() ? string.Join(Environment.NewLine, x.MsgLines.Skip(1)) : Guid.NewGuid().ToString())
                    .Select(x => "{{{" + 
                                 x.Count() +
                                 Environment.NewLine +
                                 string.Join(Environment.NewLine, x.Select(y=> y.Error.Timestamp + " [" + y.Error.Tag + "] " + y.MsgLines.First())) + 
                                 Environment.NewLine + 
                                 string.Join(Environment.NewLine, x.First().MsgLines.Skip(1).Take(6)
                                                                   .Concat(x.First().MsgLines.Skip(1).Skip(6).Where(y=> 
                                                                                          !y.TrimStart().StartsWith("at System.") &&
                                                                                          !y.TrimStart().StartsWith("at Microsoft.") &&
                                                                                          !y.TrimStart().StartsWith("at Swashbuckle.") &&
                                                                                          !y.TrimStart().StartsWith("at Npgsql.") &&
                                                                                          !y.TrimStart().StartsWith("at Polly.") &&
                                                                                          !y.TrimStart().StartsWith("at MySqlConnector") &&
                                                                                          !y.TrimStart().StartsWith("at MySql") &&
                                                                                          !y.TrimStart().StartsWith("at Pomelo") &&
                                                                                          true))) + 
                                 Environment.NewLine +
                                 "}}}"
                           )),
            token);
    }

    public async Task<IEnumerable<LogEntry>> MergeAsync(string logFile, CancellationToken token)
    {
        var fi = new FileInfo(logFile);
        await using var logStream = fi.OpenRead();
        return await _logParser.ParseAsync(fi.Directory!.Name, logStream, token);
    }
}
