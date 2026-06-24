using System.Runtime.InteropServices;
using X01.App.MediaImporter;

namespace X01.App.MediaImporter;

public class MountChecker
{
    public static string[] GetMountPoints()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return UnixMountChecker.GetMountPoints();
        }

        throw new PlatformNotSupportedException("This method is only supported on Linux.");
    }
}