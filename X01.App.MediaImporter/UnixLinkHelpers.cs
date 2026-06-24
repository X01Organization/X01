using Mono.Unix;
using Mono.Unix.Native;

namespace X01.App.MediaImporter;

public static class UnixLinkHelpers
{
    public static bool IsSymbolicLink(string path)
    {
        Stat st = GetLstat(path);

        bool isSymbolicLink = (st.st_mode & FilePermissions.S_IFMT) == FilePermissions.S_IFLNK;

        return isSymbolicLink;
    }

    public static bool IsSameFile(string path1, string path2)
    {
        Stat st1 = GetStat(path1);
        Stat st2 = GetStat(path2);

        bool isSameFile = st1.st_dev == st2.st_dev && st1.st_ino == st2.st_ino;

        return isSameFile;
    }

    private static Stat GetLstat(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Path cannot be null or whitespace.", nameof(path));
        }

        if (Syscall.lstat(path, out Stat st) != 0)
        {
            throw new InvalidOperationException($"lstat failed for path: {path}");
        }

        return st;
    }

    private static Stat GetStat(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Path cannot be null or whitespace.", nameof(path));
        }

        if (Syscall.stat(path, out Stat st) != 0)
        {
            throw new InvalidOperationException($"stat failed for path: {path}");
        }

        return st;
    }
}



