using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace X01.App.MediaImporter
{
    internal static class LinkHelpers
    {
        public static bool IsLink(string path)
        {
            return IsSymbolicLink(path) || IsHardLink(path);
        }

        public static bool IsSymbolicLink(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return WindowsLinkHelpers.IsSymbolicLink(path);
            }
            else
            {
                return UnixLinkHelpers.IsSymbolicLink(path);
            }
        }

        public static bool IsHardLink(string path)
        { 
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return WindowsLinkHelpers.IsHardLink(path);
            }
            else
            {
                return UnixLinkHelpers.IsHardLink(path);
            }
        }
    }
}
