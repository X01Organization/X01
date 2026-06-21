using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace X01.App.MediaImporter
{
    internal static class WindowsLinkHelpers
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct BY_HANDLE_FILE_INFORMATION
        {
            public uint dwFileAttributes;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
            public uint dwVolumeSerialNumber;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            public uint nNumberOfLinks;
            public uint nFileIndexHigh;
            public uint nFileIndexLow;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern SafeFileHandle CreateFileW(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetFileInformationByHandle(SafeFileHandle hFile, out BY_HANDLE_FILE_INFORMATION lpFileInformation);

        public static bool IsSymbolicLink(string path)
        {
            try
            {
                var attrs = System.IO.File.GetAttributes(path);
                return (attrs & System.IO.FileAttributes.ReparsePoint) != 0;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsHardLink(string path)
        {
            try
            {
                const uint FILE_READ_ATTRIBUTES = 0x80;
                const uint FILE_SHARE_READ = 1;
                const uint FILE_SHARE_WRITE = 2;
                const uint FILE_SHARE_DELETE = 4;
                const uint OPEN_EXISTING = 3;
                const uint FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;

                using SafeFileHandle handle = CreateFileW(path, FILE_READ_ATTRIBUTES, FILE_SHARE_READ | FILE_SHARE_WRITE | FILE_SHARE_DELETE, IntPtr.Zero, OPEN_EXISTING, FILE_FLAG_BACKUP_SEMANTICS, IntPtr.Zero);
                if (handle == null || handle.IsInvalid) return false;

                if (!GetFileInformationByHandle(handle, out BY_HANDLE_FILE_INFORMATION info)) return false;

                return info.nNumberOfLinks > 1;
            }
            catch
            {
                return false;
            }
        }
    }
}
