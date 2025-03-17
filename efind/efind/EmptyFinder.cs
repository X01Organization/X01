namespace efind
{
    public class EmptyFinder
    {
          private readonly string[] _specialDirectories =
            new[] {"lost+found", "$RECYCLE.BIN", "System Volume Information",};

        private readonly string[] _inputDirectories;
        private readonly string _outputDirectory;

        public EmptyFinder(Options options)
        {
            _inputDirectories = options.Inputs
                                           .Select(x => x.Trim())
                                           .Where(x => !string.IsNullOrEmpty(x))
                                           .Distinct()
                                           .ToArray();
            _outputDirectory = options.Output.Trim();
            Console.WriteLine("Input directories:");
            Console.WriteLine(string.Join(Environment.NewLine, _inputDirectories.Select(x => "\t" + x)));
            Console.WriteLine("Output directory:");
            Console.WriteLine("\t" + _outputDirectory);
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

        private IEnumerable<FileInfo> TryEnumerateFilesInAllDirectories(DirectoryInfo di)
        {
            if (di.Exists)
            {
                if (_specialDirectories.Contains(di.Name))
                {
                    Console.WriteLine($"Skip special directory \"{di.FullName}\"");
                }
                else
                {
                    foreach (FileInfo x in TryEnumerateFilesInTopDirectory(di))
                    {
                        yield return x;
                    }

                    foreach (DirectoryInfo x in TryEnumerateDirectoriesInTopDirectory(di))
                    {
                        foreach (FileInfo y in TryEnumerateFilesInAllDirectories(x))
                        {
                            yield return y;
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"not found the directory \"{di.FullName}\"");
            }
        }

        public void DoJob()
        {
            IEnumerable<FileInfo> allFileInfos = _inputDirectories.Select(x => new DirectoryInfo(x))
                                                    .SelectMany(TryEnumerateFilesInAllDirectories);

            if (0 < _searchingExtensions.Length)
            {
                allFileInfos = allFileInfos.Where(x => _searchingExtensions.Contains(x.Extension.ToLower()));
            }

            foreach (IGrouping<long, FileInfo>? fileInfosWithSameSize in allFileInfos.GroupBy(x => x.Length).OrderBy(x => x.Key))
            {
                try
                {
                    MoveDuplicatedFiles(FindDuplicatedFiles(fileInfosWithSameSize));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("error by moving and finding duplicated files for size: " +
                                      fileInfosWithSameSize.Key +
                                      "\n" + ex);
                }
            }
        }

        private List<FileInfo> FindDuplicatedFiles(IEnumerable<FileInfo> fileInfosWithSameSize)
        {
            List<FileInfo> uniqueFiles = new();
            List<FileInfo> duplicatedFiles = new();
            foreach (FileInfo x in fileInfosWithSameSize)
            {
                Console.WriteLine("comparing " + x.FullName);
                if (uniqueFiles.Contains(x))
                {
                    throw new Exception(x.FullName + " already exists in uniqueFiles");
                }

                if (duplicatedFiles.Contains(x))
                {
                    throw new Exception(x.FullName + " already exists in duplicatedFiles");
                }

                var exist = uniqueFiles.Select((y, i) => new
                {
                    File = y,
                    Index = i,
                }).FirstOrDefault(y => IsSame(x, y.File));
                if (null == exist)
                {
                    uniqueFiles.Add(x);
                }
                else
                {
                    if (CompareFilesByDateTime(exist.File, x))
                    {
                        uniqueFiles[exist.Index] = x;
                        duplicatedFiles.Add(exist.File);
                    }
                    else
                    {
                        duplicatedFiles.Add(x);
                    }

                    if (1 == duplicatedFiles.Count)
                    {
                        new DirectoryInfo(_outputDirectory).Create();
                    }
                }
            }

            return duplicatedFiles;
        }

        private void MoveDuplicatedFiles(IEnumerable<FileInfo> duplicatedFiles)
        {
            foreach (FileInfo x in duplicatedFiles)
            {
                Console.WriteLine("moving " + x.FullName);
                string newFullName = GetUniqueName(new DirectoryInfo(_outputDirectory).FullName,
                    Path.GetFileNameWithoutExtension(x.Name), x.Extension);
                x.MoveTo(newFullName);
            }
        }

        private string GetUniqueName(string dir, string name, string ext)
        {
            int i = 0;
            while (i < int.MaxValue)
            {
                string newName = 0 == i ? name : name + "_" + String.Format("{0,5:0000000}", i);
                string newFullName = Path.Combine(dir, newName + ext);
                if (!File.Exists(newFullName))
                {
                    return newFullName;
                }

                ++i;
            }

            throw new Exception("Can not get a unique name for " + name + ext);
        }
    }
}
