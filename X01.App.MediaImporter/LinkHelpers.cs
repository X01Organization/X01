using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace X01.App.MediaImporter;

internal static class LinkHelpers
{
    public static bool IsSymbolicLink(string path)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return UnixLinkHelpers.IsSymbolicLink(path);
        }

        throw new PlatformNotSupportedException("Symbolic link detection is only supported on Linux.");
    }
}
