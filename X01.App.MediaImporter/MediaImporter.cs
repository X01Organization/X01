using System.Diagnostics;
using System.Globalization;

namespace X01.App.MediaImporter;
public class MediaImporter
{
    private readonly string[] _specialDirectories =
          new[] { "lost+found", "$RECYCLE.BIN", "System Volume Information", };
    private readonly HashSet<string> _notInodes  = new();

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

        await ImportAsync(inputs, outputDirectoryInfo, option.Extensions ?? new List<string>(), token);
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

    private IEnumerable<DirectoryInfo> TryEnumerateDirectoriesInTopDirectory(DirectoryInfo di)
    {
        try
        {
            return di.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error by TryEnumerateDirectoriesInTopDirectory({di.FullName}):");
            Console.WriteLine(ex.ToString());
            return Enumerable.Empty<DirectoryInfo>();
        }
    }

    private IEnumerable<FileInfo> TryEnumerateFilesInTopDirectory(DirectoryInfo di)
    {
        try
        {
            return di.EnumerateFiles("*", SearchOption.TopDirectoryOnly)
                     .Where(x => x.Length > -1)
                     .ToArray();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error by TryEnumerateFilesInTopDirectory({di.FullName}):");
            Console.WriteLine(ex.ToString());
            return Enumerable.Empty<FileInfo>();
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

    private async Task ImportAsync(FileSystemInfo[] inputFileSystemInfos, DirectoryInfo outputDirectoryInfo, List<string> extensions, CancellationToken token)
    {
        IEnumerable<FileInfo> allInputFiles = GetAllInputImageFileInfos(inputFileSystemInfos, outputDirectoryInfo, extensions);

        foreach (IGrouping<long, FileInfo>? fileInfosWithSameSize in allInputFiles.GroupBy(x => x.Length).OrderBy(x => x.Key))
        {
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

    private IEnumerable<FileInfo> GetAllInputImageFileInfos(FileSystemInfo[] inputFileSystemInfos, DirectoryInfo outputDirectoryInfo, List<string> extensions)
    {
        foreach (FileSystemInfo? x in inputFileSystemInfos.DistinctBy(x=> x.FullName))
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
        return inputFileSystemInfo.FullName.StartsWith(outputDirectoryInfo.FullName);
    }

    private bool IsGoodSizeImage(FileInfo fileInfo, List<string> extensions)
    {
        if ((1024 / 2) * 1024 > fileInfo.Length)
        {
            // > 0.5MB
            return false;
        }

        if (0 < extensions.Count && !extensions.Contains(fileInfo.Extension.ToLowerInvariant()))
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
            if (uniqueFiles.Select(y=> y.FullName).Contains(x.FullName))
            {
                throw new Exception(x.FullName + " already exists in uniqueFiles");
            }

            FileInfo? existFile = uniqueFiles.FirstOrDefault(y => IsSame(x, y));

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

            if(ShouldbeXXXX(x, dateTime))
            { 
                 outputDir = Path.Combine(outputDir, "XXXX");
            }

            outputDir = Path.Combine(outputDir, folder , dateTime.ToString("MM"));

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
        if(!nameWithoutExt.StartsWith("IMG_", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        string maybeDateTimeName = nameWithoutExt.Substring(4);

        if (!DateTime.TryParseExact(maybeDateTimeName, "yyyyMMdd_HHmmssfff", CultureInfo.CurrentCulture, DateTimeStyles.None, 
            out DateTime maybeDateTime))
        {
            return false;
        }

        if(maybeDateTime.Year == minDateTime.Year)
        {
            return  false;
        }

        return true;
    }

    private string GetUniqueName(string dir, string name, string ext)
    {
        int i = 0;
        while (i < int.MaxValue)
        {
            string newName = 0 == i ? name : name + "_" + string.Format("{0,5:0000000}", i);
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
        if(fi1.FullName == fi2.FullName)
        {
            throw new UnreachableException("same file");
        }

        if(fi1.Directory!.FullName == fi2.Directory!.FullName)
        {
            return;
        }

        string inode = $"{fi1.Directory!.FullName} <> {fi2.Directory!.FullName}";
        if(_notInodes.Contains(inode))
        {
            return;
        }

        string testFile1 = Path.Combine(fi1.Directory!.FullName, "dummy.zhichaoxiang.test.inode.file");
        if(File.Exists(testFile1))
        {
            FileInfo ss = new(testFile1);
           ss.Delete();
        }
        string testFile2 = Path.Combine(fi2.Directory!.FullName, "dummy.zhichaoxiang.test.inode.file");
        if(File.Exists(testFile2))
        {
            FileInfo ss = new(testFile2);
            ss.Delete();
        }
        File.WriteAllText(testFile1, "1");

        if(!File.Exists(testFile1))
        { 
            throw new UnreachableException($"2: same inode: {fi1.Directory!.FullName}  and {fi2.Directory!.FullName}");
        }

        if(File.Exists(testFile2))
        { 
            throw new UnreachableException($"3: same inode: {fi1.Directory!.FullName}  and {fi2.Directory!.FullName}");
        }

        FileInfo ss1 = new(testFile1);
        ss1.Delete();

        bool added = _notInodes.Add(inode);
        if(!added)
        {
            throw new UnreachableException($"4: same inode: {fi1.Directory!.FullName}  and {fi2.Directory!.FullName}");
        }
    }

    private bool IsSame(FileInfo fi1, FileInfo fi2)
    {
        if (fi1.FullName == fi2.FullName)
        {
            return true;
        }

        try
        {
            using (FileStream s1 = fi1.OpenRead())
            {
                using (FileStream s2 = fi2.OpenRead())
                {
                    while (true)
                    {
                        int b1 = s1.ReadByte();
                        int b2 = s2.ReadByte();
                        if (b1 != b2)
                        {
                            return false;
                        }

                        if (-1 == b1)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error by comparing:");
            Console.WriteLine(ex.ToString());
            return false;
        }
    }
}
