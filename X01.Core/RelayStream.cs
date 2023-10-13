using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Journaway.Core.Xiang;

public class RelayStream : Stream
{
    private readonly ILogger _logger;
    private readonly Stream _stream;

    public override bool CanRead
    {
        get
        {
            var v = _stream.CanRead;
            LogInformation(v);
            return v;
        }
    }

    public override bool CanSeek
    {
        get
        {
            var v = _stream.CanSeek;
            LogInformation(v);
            return v;
        }
    }

    public override bool CanWrite
    {
        get
        {
            var v = _stream.CanWrite;
            LogInformation(v);
            return v;
        }
    }

    public override long Length
    {
        get
        {
            var v = _stream.Length;
            LogInformation(v);
            return v;
        }
    }

    public override long Position
    {
        get
        {
            var v = _stream.Position;
            LogInformation(v, "GetPosition");
            return v;
        }

        set
        {
            var v = value;
            LogInformation(v, "SetPosition");
            _stream.Position = v;
        }
    }

    public override void Flush()
    {
        LogInformation(_stream.Position);
        _stream.Flush();
    }

    public RelayStream(ILogger logger, Stream stream)
    {
        _logger = logger;
        _stream = stream;
    }

    public RelayStream(Stream stream)
    {
        _stream = stream;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        int len = _stream.Read(buffer, offset, count);
        LogInformation($"{offset}<>{count}<>{len}");
        return len;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        long position = _stream.Seek(offset, origin);
        LogInformation($"{offset}<>{origin}<>{position}");
        return position;
    }

    public override void SetLength(long value)
    {
        LogInformation($"{value}");
        _stream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        LogInformation($"{offset}<>{count}");
        _stream.Write(buffer, offset, count);
    }

    protected override void Dispose(bool disposing)
    {
        LogInformation(_stream.Position);
        base.Dispose(disposing);
    }

    public override ValueTask DisposeAsync()
    {
        LogInformation(_stream.Position);
        return base.DisposeAsync();
    }

    private void LogInformation<TValue>(TValue value, [CallerMemberName] string callerMemberName = "")
    {
        _logger.LogInformation($"{_stream.GetHashCode()}  {callerMemberName}-->{value}");
    }
}