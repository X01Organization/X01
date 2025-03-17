using Microsoft.Win32.SafeHandles;

namespace X01.App.CmdLine.ArgsParser.Test;
public class testStream : FileStream
{
    public testStream(SafeFileHandle handle, FileAccess access) : base(handle, access)
    {
    }

    public testStream(nint handle, FileAccess access) : base(handle, access)
    {
    }

    public testStream(string path, FileMode mode) : base(path, mode)
    {
    }

    public testStream(string path, FileStreamOptions options) : base(path, options)
    {
    }

    public testStream(SafeFileHandle handle, FileAccess access, int bufferSize) : base(handle, access, bufferSize)
    {
    }

    public testStream(nint handle, FileAccess access, bool ownsHandle) : base(handle, access, ownsHandle)
    {
    }

    public testStream(string path, FileMode mode, FileAccess access) : base(path, mode, access)
    {
    }

    public testStream(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync) : base(handle, access, bufferSize, isAsync)
    {
    }

    public testStream(nint handle, FileAccess access, bool ownsHandle, int bufferSize) : base(handle, access, ownsHandle, bufferSize)
    {
    }

    public testStream(string path, FileMode mode, FileAccess access, FileShare share) : base(path, mode, access, share)
    {
    }

    public testStream(nint handle, FileAccess access, bool ownsHandle, int bufferSize, bool isAsync) : base(handle, access, ownsHandle, bufferSize, isAsync)
    {
    }

    public testStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize) : base(path, mode, access, share, bufferSize)
    {
    }

    public testStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync) : base(path, mode, access, share, bufferSize, useAsync)
    {
    }

    public testStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options) : base(path, mode, access, share, bufferSize, options)
    {
    }

    public override ValueTask DisposeAsync()
    {
        Console.WriteLine("============> DisposeAsync");
        return base.DisposeAsync();
    }
    protected override void Dispose(bool disposing)
    {
        Console.WriteLine($"============> Dispose({disposing})");
        base.Dispose(disposing);
    }
}
