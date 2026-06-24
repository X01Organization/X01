using System.Runtime.InteropServices;

namespace X01.App.MediaImporter;
public static class UnixMountChecker
{
    public static string[] GetMountPoints()
    {
if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux) )
            {
            throw new PlatformNotSupportedException("This method is only supported on Linux.");
            }

     string [] mountPoints = File.ReadLines("/proc/self/mountinfo")
            .Select(ParseMountPoint)
            .Where(p => !string.IsNullOrEmpty(p))
            .Cast<string>()
            .Distinct()
            .ToArray();

            return mountPoints;
    }

    private static string? ParseMountPoint(string line)
    {
        // mountinfo format: mountID parentID dev root mountPoint ...
        // fields are space-separated but spaces in paths are escaped as \040 etc.

        var parts = line.Split(' ', 6);
        if (parts.Length < 5)
            return null;

        return UnescapeMountField(parts[4]);
    }

    private static string UnescapeMountField(string value)
    {
        // minimal mountinfo unescape handling
        return value
            .Replace("\\040", " ")
            .Replace("\\011", "\t")
            .Replace("\\012", "\n")
            .Replace("\\134", "\\");
    }
}