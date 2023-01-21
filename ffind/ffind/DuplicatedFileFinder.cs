namespace ffind
{
    public class DuplicatedFileFinder
    {
        private readonly string[] _searchingDirectories;
        private readonly string[] _searchingExtensions;
        private readonly string _duplicatedFileDirectory;

        public DuplicatedFileFinder(Options options)
        {
            _searchingDirectories = options.Inputs
                                           .Select(x => x.Trim())
                                           .Where(x => !string.IsNullOrEmpty(x))
                                           .Distinct()
                                           .ToArray();
            _searchingExtensions = options.Extensions
                                         ?.Select(x => x.Trim())
                                          .Where(x => !string.IsNullOrEmpty(x))
                                          .Distinct()
                                          .Select(x => "." + x.ToLower())
                                          .ToArray() ?? Array.Empty<string>();
            _duplicatedFileDirectory = options.Output.Trim();
            Console.WriteLine("Input directories:");
            Console.WriteLine(string.Join(Environment.NewLine, _searchingDirectories.Select(x => "\t" + x)));
            Console.WriteLine("Output directory:");
            Console.WriteLine("\t" + _duplicatedFileDirectory);
            Console.WriteLine("Extensions:");
            Console.WriteLine(string.Join(Environment.NewLine, _searchingExtensions.Select(x => "\t" + x)));
        }

        public void DoJob()
        {
            var allFileInfos = _searchingDirectories.Select(x => new DirectoryInfo(x))
                                                    .Where(x => x.Exists)
                                                    .SelectMany(x
                                                         =>
                                                     {
                                                         try
                                                         {
                                                             var files = x.EnumerateFiles("*",
                                                                 SearchOption.AllDirectories);
                                                             return files;
                                                         }
                                                         catch (Exception ex)
                                                         {
                                                             Console.WriteLine("Error by EnumerateFiles:");
                                                             Console.WriteLine(ex.ToString());
                                                             return Enumerable.Empty<FileInfo>();
                                                         }
                                                     })
                //.Where(x =>
                // {
                //     try
                //     {
                //         return x.Length > -1;
                //     }
                //     catch (Exception ex)
                //     {
                //         Console.WriteLine("Error by get file length:");
                //         Console.WriteLine(ex.ToString());
                //         return false;
                //     }
                // })
                ;
            if (0 < _searchingExtensions.Length)
            {
                allFileInfos = allFileInfos.Where(x => _searchingExtensions.Contains(x.Extension.ToLower()));
            }

            foreach (var fileInfosWithSameSize in allFileInfos.GroupBy(x => x.Length).OrderBy(x => x.Key))
            {
                MoveDuplicatedFiles(FindDuplicatedFiles(fileInfosWithSameSize));
            }
        }

        private List<FileInfo> FindDuplicatedFiles(IEnumerable<FileInfo> fileInfosWithSameSize)
        {
            List<FileInfo> uniqueFiles = new List<FileInfo>();
            List<FileInfo> duplicatedFiles = new List<FileInfo>();
            foreach (var x in fileInfosWithSameSize)
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
                        new DirectoryInfo(_duplicatedFileDirectory).Create();
                    }
                }
            }

            return duplicatedFiles;
        }

        private void MoveDuplicatedFiles(IEnumerable<FileInfo> duplicatedFiles)
        {
            foreach (var x in duplicatedFiles)
            {
                Console.WriteLine("moving " + x.FullName);
                var newFullName = GetUniqueName(new DirectoryInfo(_duplicatedFileDirectory).FullName,
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

        private bool CompareFilesByDateTime(FileInfo existFile, FileInfo foundFile)
        {
            if (existFile.CreationTime == foundFile.CreationTime)
            {
                if (existFile.LastWriteTime == foundFile.LastWriteTime)
                {
                    return existFile.LastAccessTime > foundFile.LastAccessTime;
                }
                else
                {
                    return existFile.LastWriteTime > foundFile.LastWriteTime;
                }
            }
            else
            {
                if (existFile.LastWriteTime == foundFile.LastWriteTime)
                {
                    return existFile.CreationTime > foundFile.CreationTime;
                }
                else
                {
                    return GetFileMinDateTime(existFile) > GetFileMinDateTime(foundFile);
                }
            }
        }

        private DateTime GetFileMinDateTime(FileInfo fi)
        {
            if (fi.CreationTime > fi.LastWriteTime)
            {
                return fi.LastWriteTime;
            }

            return fi.CreationTime;
        }

        private bool IsSame(FileInfo fi1, FileInfo fi2)
        {
            if (fi1.FullName == fi2.FullName)
            {
                return true;
            }

            try
            {
                using (var s1 = fi1.OpenRead())
                {
                    using (var s2 = fi2.OpenRead())
                    {
                        while (true)
                        {
                            var b1 = s1.ReadByte();
                            var b2 = s2.ReadByte();
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
}