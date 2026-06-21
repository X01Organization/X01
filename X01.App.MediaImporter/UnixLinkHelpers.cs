using System;
using System.Runtime.InteropServices;

namespace X01.App.MediaImporter
{
    // Minimal Unix helpers to detect symbolic links and compare inodes.
    // Currently intended for Linux/macOS. Uses libc stat/lstat via P/Invoke.
    internal static class UnixLinkHelpers
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct Timespec { public long tv_sec; public long tv_nsec; }

        [StructLayout(LayoutKind.Sequential)]
        private struct Stat
        {
            public ulong st_dev;
            public ulong st_ino;
            public ulong st_nlink;
            public uint st_mode;
            public uint st_uid;
            public uint st_gid;
            public int __pad0;
            public ulong st_rdev;
            public long st_size;
            public long st_blksize;
            public long st_blocks;
            public Timespec st_atim;
            public Timespec st_mtim;
            public Timespec st_ctim;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public long[] __glibc_reserved;
        }

        [DllImport("libc", SetLastError = true)]
        private static extern int lstat(string path, out Stat buf);

        [DllImport("libc", SetLastError = true)]
        private static extern int stat(string path, out Stat buf);

        private const uint S_IFMT = 0xF000; // bit mask for the file type bitfields
        private const uint S_IFLNK = 0xA000; // symbolic link

        // Return true if the path is a symbolic link (lstat is used so symlink itself is inspected)
        public static bool IsSymbolicLink(string path)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && !RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return false;
            }

            if (lstat(path, out Stat st) != 0)
            {
                return false;
            }

            return (st.st_mode & S_IFMT) == S_IFLNK;
        }

        // Try to get device and inode (stat follows symlinks). Returns false on error.
        public static bool TryGetInode(string path, out ulong dev, out ulong ino, out ulong nlink)
        {
            dev = ino = nlink = 0;
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && !RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                throw new InvalidOperationException("UnixLinkHelpers is only supported on Linux and OSX.");
            }

            if (stat(path, out Stat st) != 0)
            {
                return false;
            }

            dev = st.st_dev;
            ino = st.st_ino;
            nlink = st.st_nlink;
            return true;
        }

        // Return true when two paths refer to the same underlying inode (device+inode match).
        public static bool AreSameInode(string path1, string path2)
        {
            if (!TryGetInode(path1, out ulong d1, out ulong i1, out _)) return false;
            if (!TryGetInode(path2, out ulong d2, out ulong i2, out _)) return false;
            return d1 == d2 && i1 == i2;
        }

        // Return true if file has more than one hard link (nlink > 1).
        public static bool HasMultipleHardLinks(string path)
        {
            if (!TryGetInode(path, out _, out _, out ulong nlink)) return false;
            return nlink > 1;
        }

        // Alias for compatibility
        public static bool IsHardLink(string path)
        {
            return HasMultipleHardLinks(path);
        }
    }
}
