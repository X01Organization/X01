namespace refactorDiffChecker
{
    public class FileChanges
    {
        public Changes FilenameChanges { get; }
        public List<Changes> Changes { get; } = new List<Changes>();

        public FileChanges(Changes filenameChanges)
        {
            FilenameChanges = filenameChanges;
        }
    }
}