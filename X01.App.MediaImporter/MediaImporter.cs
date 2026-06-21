using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace X01.App.MediaImporter;

public class MediaImporter
{
    private readonly FileContentComparer _fileContentComparer = new();
    private readonly string[] _specialDirectories =
          new[] { "lost+found", "$RECYCLE.BIN", "System Volume Information", };
    // Use OS-appropriate comparer for path keys (Windows is case-insensitive).
    private readonly HashSet<string> _notInodes = new(
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? StringComparer.OrdinalIgnoreCase
            : StringComparer.Ordinal);
    public async Task ImportAsync(Option option, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(option.OutputDirectory))
        {
            Console.WriteLine("missing the output directory!");
            return;
        }

        DirectoryInfo outputDirectoryInfo = new(option.OutputDirectory);

        FileSystemInfo[] inputs = GetInputFileSystemInfos(option.InputFilesOrDirectories).ToArray();
        if (1 > inputs.Length)
        {
            Console.WriteLine("missing the input files or directories!");
            return;
        }

        inputs = inputs.Where(x => !IsOutputDirectory(x, outputDirectoryInfo)).ToArray();

        if (1 > inputs.Length)
        {
            Console.WriteLine("the input files or directories can not be same like output diriectory!");
            return;
        }

        if (!outputDirectoryInfo.Exists)
        {
            outputDirectoryInfo.Create();
        }

        Console.WriteLine("Input files or directories:");
        Console.WriteLine(string.Join(Environment.NewLine, inputs.Select(x => "\t" + x.FullName)));
        Console.WriteLine("Output directory:");
        Console.WriteLine("\t" + outputDirectoryInfo.FullName);

        // Normalize extensions once to a HashSet of lowercase values (including leading dot)
        List<string> extList = option.Extensions ?? new List<string>();
        HashSet<string> extSet = new(extList
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.StartsWith('.') ? x.ToLowerInvariant() : ("." + x.ToLowerInvariant())));

        await ImportAsync(inputs, outputDirectoryInfo, extSet, token);
    }

    private IEnumerable<FileSystemInfo> GetInputFileSystemInfos(IEnumerable<string>? inputFilesOrDirectories)
    {
        if (null == inputFilesOrDirectories)
        {
            yield break;
        }

        foreach (string x in inputFilesOrDirectories)
        {
            if (File.Exists(x))
            {
                yield return new FileInfo(x);
            }
            if (Directory.Exists(x))
            {
                yield return new DirectoryInfo(x);
            }
        }
    }

    private DirectoryInfo[] TryEnumerateDirectoriesInTopDirectory(DirectoryInfo di)
    {
        try
        {
            // Materialize the enumeration here so exceptions caused by IO
            // during enumeration are caught by this try/catch.
            return di.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).ToArray();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error by TryEnumerateDirectoriesInTopDirectory({di.FullName}):");
            Console.WriteLine(ex.ToString());
            return Array.Empty<DirectoryInfo>();
        }
    }

    private FileInfo[] TryEnumerateFilesInTopDirectory(DirectoryInfo di)
    {
        try
        {
            var list = new List<FileInfo>();
            foreach (FileInfo f in di.EnumerateFiles("*", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    // Force access to Length so IO/permission errors are caught here.
                    _ = f.Length;
                    list.Add(f);
                }
                catch (Exception exFile)
                {
                    Console.WriteLine($"Warning: cannot access file {f.FullName}: {exFile.Message}");
                    // skip this file and continue with others
                }
            }

            return list.ToArray();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error by TryEnumerateFilesInTopDirectory({di.FullName}):");
            Console.WriteLine(ex.ToString());
            return Array.Empty<FileInfo>();
        }
    }

    private IEnumerable<FileInfo> TryEnumerateFilesInAllDirectories(DirectoryInfo di, DirectoryInfo outputDirectoryInfo)
    {
        if (!di.Exists)
        {
            Console.WriteLine($"not found the directory \"{di.FullName}\"");
            yield break;
        }

        if (_specialDirectories.Contains(di.Name))
        {
            Console.WriteLine($"Skip special directory \"{di.FullName}\"");
            yield break;
        }

        foreach (FileInfo x in TryEnumerateFilesInTopDirectory(di))
        {
            yield return x;
        }

        foreach (DirectoryInfo x in TryEnumerateDirectoriesInTopDirectory(di))
        {
            if (IsOutputDirectory(x, outputDirectoryInfo))
            {
                continue;
            }

            foreach (FileInfo y in TryEnumerateFilesInAllDirectories(x, outputDirectoryInfo))
            {
                yield return y;
            }
        }
    }

    private async Task ImportAsync(FileSystemInfo[] inputFileSystemInfos, DirectoryInfo outputDirectoryInfo, HashSet<string> extensions, CancellationToken token)
    {
        IEnumerable<FileInfo> allInputFiles = GetAllInputImageFileInfos(inputFileSystemInfos, outputDirectoryInfo, extensions);

        foreach (IGrouping<long, FileInfo>? fileInfosWithSameSize in allInputFiles.GroupBy(x => x.Length).OrderBy(x => x.Key))
        {
            token.ThrowIfCancellationRequested();
            try
            {
                List<FileInfo> resultFiles = RemoveDuplicatedFiles(fileInfosWithSameSize);
                MoveToOutputDirectory(resultFiles, outputDirectoryInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error by moving and finding duplicated files for size: " +
                                  fileInfosWithSameSize.Key +
                                  "\n" + ex);
            }
        }
    }

    private IEnumerable<FileInfo> GetAllInputImageFileInfos(FileSystemInfo[] inputFileSystemInfos, DirectoryInfo outputDirectoryInfo, HashSet<string> extensions)
    {
        foreach (FileSystemInfo? x in inputFileSystemInfos.DistinctBy(x => x.FullName))
        {
            if (x is FileInfo inputFileInfo)
            {
                if (IsGoodSizeImage(inputFileInfo, extensions))
                {
                    yield return inputFileInfo;
                }
            }

            if (x is DirectoryInfo inputDirectoryInfo)
            {
                foreach (FileInfo y in TryEnumerateFilesInAllDirectories(inputDirectoryInfo, outputDirectoryInfo))
                {
                    if (IsGoodSizeImage(y, extensions))
                    {
                        yield return y;
                    }
                }
            }
        }
    }

    private bool IsOutputDirectory(FileSystemInfo inputFileSystemInfo, DirectoryInfo outputDirectoryInfo)
    {
        // Normalize both paths and ensure we compare directory-prefixes safely.
        // Use GetFullPath to resolve any relative segments. Append a directory
        // separator after trimming so comparisons like "/path/out" vs
        // "/path/out2" don't falsely match. Using Path.GetFullPath also keeps
        // file paths intact — a file inside the output folder will still start
        // with the normalized output folder path.
        string inputFull = Path.GetFullPath(inputFileSystemInfo.FullName);
        string outputFull = Path.GetFullPath(outputDirectoryInfo.FullName);

        inputFull = inputFull.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
        outputFull = outputFull.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;

        StringComparison cmp = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;

        return inputFull.StartsWith(outputFull, cmp);
    }

    private bool IsGoodSizeImage(FileInfo fileInfo, HashSet<string> extensions)
    {
        const long MinBytes = 512 * 1024; // 0.5 MB
        if (fileInfo.Length < MinBytes)
        {
            return false;
        }

        if (extensions.Count > 0 && !extensions.Contains(fileInfo.Extension.ToLowerInvariant()))
        {
            return false;
        }

        return true;
    }

    private List<FileInfo> RemoveDuplicatedFiles(IEnumerable<FileInfo> fileInfosWithSameSize)
    {
        List<FileInfo> uniqueFiles = new();
        foreach (FileInfo x in fileInfosWithSameSize)
        {
            if (uniqueFiles.Select(y => y.FullName).Contains(x.FullName))
            {
                throw new Exception(x.FullName + " already exists in uniqueFiles");
            }

            FileInfo? existFile = uniqueFiles.FirstOrDefault(
                y => _fileContentComparer.MatchesByContent(x, y));

            if (null == existFile)
            {
                uniqueFiles.Add(x);
            }
            else
            {
                ThrowIfSameInode(existFile, x);

                DateTime minDatetime = GetFileMinDateTime(existFile, x);
                if (minDatetime < existFile.LastWriteTime)
                {
                    File.SetLastWriteTime(existFile.FullName, minDatetime);
                }

                Console.WriteLine("deleting " + x.FullName);
                x.Delete();
            }
        }

        return uniqueFiles;
    }

    private void MoveToOutputDirectory(IEnumerable<FileInfo> results, DirectoryInfo outputDirectoryInfo)
    {
        foreach (FileInfo x in results)
        {
            if (!x.Exists)
            {
                continue;
            }

            DateTime dateTime = GetFileMinDateTime(x);

            string folder = dateTime.Year.ToString();

            string outputDir = outputDirectoryInfo.FullName;

            if (ShouldbeXXXX(x, dateTime))
            {
                outputDir = Path.Combine(outputDir, "XXXX");
            }

            outputDir = Path.Combine(outputDir, folder, dateTime.ToString("MM"));

            DirectoryInfo outputdirinfo = new(outputDir);
            if (outputdirinfo.Exists)
            {
                RemoveDuplicatedFiles(outputdirinfo.GetFiles().Where(y => y.Length == x.Length).Append(x));
            }

            if (!x.Exists)
            {
                continue;
            }

            string newFullName = GetUniqueName(outputDir, Path.GetFileNameWithoutExtension(x.Name), x.Extension);
            FileInfo targetFileInfo = new(newFullName);
            if (!targetFileInfo.Directory!.Exists)
            {
                targetFileInfo.Directory.Create();
            }

            Console.WriteLine("moving " + x.FullName);
            x.MoveTo(targetFileInfo.FullName);
        }
    }

    private bool ShouldbeXXXX(FileInfo fi, DateTime minDateTime)
    {
        string nameWithoutExt = Path.GetFileNameWithoutExtension(fi.Name);
        if (!nameWithoutExt.StartsWith("IMG_", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        string maybeDateTimeName = nameWithoutExt.Substring(4);

        if (!DateTime.TryParseExact(maybeDateTimeName, "yyyyMMdd_HHmmssfff", CultureInfo.CurrentCulture, DateTimeStyles.None,
            out DateTime maybeDateTime))
        {
            return false;
        }

        if (maybeDateTime.Year == minDateTime.Year)
        {
            return false;
        }

        return true;
    }

    private string GetUniqueName(string dir, string name, string ext)
    {
        int i = 0;
        while (i < int.MaxValue)
        {
            string newName = 0 == i ? name : name + "_" + i.ToString("D7");
            string newFullName = Path.Combine(dir, newName + ext);
            if (!File.Exists(newFullName))
            {
                return newFullName;
            }

            ++i;
        }

        throw new Exception("Can not get a unique name for " + name + ext);
    }

    private DateTime GetFileMinDateTime(FileInfo fi)
    {
        return new[] { fi.CreationTime, fi.LastWriteTime, fi.LastAccessTime, DateTime.Today, }.Where(x => x != DateTime.MinValue).Min();
    }

    private DateTime GetFileMinDateTime(FileInfo fi1, FileInfo fi2)
    {
        return new[] {
            fi1.CreationTime,
            fi1.LastWriteTime,
            fi1.LastAccessTime,
            fi2.CreationTime,
            fi2.LastWriteTime,
            fi2.LastAccessTime,
            DateTime.Today, }
        .Where(x => x != DateTime.MinValue)
        .Min();
    }

    private void ThrowIfSameInode(FileInfo fi1, FileInfo fi2)
    {
        if (fi1.FullName == fi2.FullName)
        {
            throw new UnreachableException("same file");
        }

        if (fi1.Directory!.FullName == fi2.Directory!.FullName)
        {
            return;
        }

        string inode = $"{fi1.Directory!.FullName} <> {fi2.Directory!.FullName}";
        if (_notInodes.Contains(inode))
        {
            return;
        }

        ThrowIfSameInode1(fi1, fi2);

        bool added = _notInodes.Add(inode);
        if (!added)
        {
            throw new UnreachableException($"4: same inode: {fi1.Directory!.FullName}  and {fi2.Directory!.FullName}");
        }
    }

    private void ThrowIfSameInode1(FileInfo fi1, FileInfo fi2)
    {
        string testFile1 = Path.Combine(fi1.Directory!.FullName, "dummy.zhichaoxiang.test.inode.file");
        if (File.Exists(testFile1))
        {
            FileInfo ss = new(testFile1);
            ss.Delete();
        }
        string testFile2 = Path.Combine(fi2.Directory!.FullName, "dummy.zhichaoxiang.test.inode.file");
        if (File.Exists(testFile2))
        {
            FileInfo ss = new(testFile2);
            ss.Delete();
        }
        File.WriteAllText(testFile1, "1");

        if (!File.Exists(testFile1))
        {
            throw new UnreachableException($"2: same inode: {fi1.Directory!.FullName}  and {fi2.Directory!.FullName}");
        }

        if (File.Exists(testFile2))
        {
            throw new UnreachableException($"3: same inode: {fi1.Directory!.FullName}  and {fi2.Directory!.FullName}");
        }

        FileInfo ss1 = new(testFile1);
        ss1.Delete();
    }
}
