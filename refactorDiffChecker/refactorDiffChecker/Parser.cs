namespace refactorDiffChecker
{
    public class Parser
    {
        public List<List<string>> GetChangedFiles(string[] lines)
        {
            List<List<string>> changedFiles = new();
            List<string> changedFile = null;
            int i = 0;
            while (lines.Length > i)
            {
                string line = lines[i];
                if (line.StartsWith("---"))
                {
                    changedFile = new List<string>();
                    changedFiles.Add(changedFile);
                    System.Diagnostics.Debug.Assert(lines.Length > 1 + i);
                    System.Diagnostics.Debug.Assert(lines[1 + i].StartsWith("+++"));
                }

                if (null != changedFile)
                {
                    if (line.StartsWith('+') || line.StartsWith('-'))
                    {
                        changedFile.Add(line);
                    }
                }

                ++i;
            }

            return changedFiles;
        }

        private bool GetChanges(ref string[] lines, out string changes)
        {
            List<string> changedLines = new();
            char changeSign = lines[0][0];
            System.Diagnostics.Debug.Assert('+' == changeSign || '-' == changeSign);
            int i = 0;
            while (lines.Length > i)
            {
                string line = lines[i];
                if (!line.StartsWith(changeSign))
                {
                    break;
                }

                changedLines.Add(line.Substring(1));
                ++i;
            }

            changes = string.Join(Environment.NewLine, changedLines);
            lines = lines.Skip(i).ToArray();
            return '-' == changeSign;
        }

        public FileChanges GetChanges(List<string> fileChanges)
        {
            System.Diagnostics.Debug.Assert(fileChanges[0].StartsWith("--- a/")
                                         || fileChanges[0].StartsWith("--- /dev/null"));
            System.Diagnostics.Debug.Assert(fileChanges[1].StartsWith("+++ b/")
                                         || fileChanges[1].StartsWith("+++ /dev/null"));
            FileChanges fc = new FileChanges(new Changes(fileChanges[0].Substring(6), fileChanges[1].Substring(6)));
            string[] lines = fileChanges.Skip(2).ToArray();
            string removed;
            string added;
            while (0 < lines.Length)
            {
                removed = null;
                bool isremoved = GetChanges(ref lines, out removed);
                if (isremoved)
                {
                    added = null;
                    if (0 < lines.Length)
                    {
                        bool isadded = !GetChanges(ref lines, out added);
                        System.Diagnostics.Debug.Assert(isadded);
                    }

                    fc.Changes.Add(new Changes(removed, added));
                }
                else
                {
                    added = removed;
                    fc.Changes.Add(new Changes(null, added));
                }
            }

            return fc;
        }

        private bool removeSameIgnore(List<Changes> ignoreChanges, ref string removed, ref string added)
        {
            foreach (Changes i in ignoreChanges)
            {
                if (removed.StartsWith(i.Removed) && added.StartsWith(i.Added))
                {
                    removed = removed.Substring(i.Removed.Length);
                    added = added.Substring(i.Added.Length);
                    return true;
                }

                if (removed.EndsWith(i.Removed) && added.EndsWith(i.Added))
                {
                    removed = removed.Substring(0, removed.Length - i.Removed.Length);
                    added = added.Substring(0, added.Length - i.Added.Length);
                    return true;
                }
            }

            return false;
        }

        private bool removeSameChars(ref string removed, ref string added)
        {
            char[] removedchars = removed.ToCharArray();
            char[] addedchars = added.ToCharArray();
            int i = 0;
            while (i < removedchars.Length && i < addedchars.Length)
            {
                if (removedchars[i] != addedchars[i])
                {
                    break;
                }

                ++i;
            }

            System.Diagnostics.Debug.Assert(i <= removedchars.Length && i <= addedchars.Length);
            removed = new string(removedchars.Skip(i).ToArray());
            added = new string(addedchars.Skip(i).ToArray());
            return i > 0;
        }

        public HashSet<Changes> CommentChanges = new();

        public Changes GetChanges(List<Changes> ignoreChanges, string removed, string added)
        {
            if (null == removed || null == added)
            {
                return new Changes(removed, added);
            }

            if (removed.TrimStart().StartsWith("//") || removed.Contains("\"") || removed.Contains("\'") ||
                added.TrimStart().StartsWith("//") || added.Contains("\"") || added.Contains("\'"))
            {
                Changes c = new Changes(removed, added);
                CommentChanges.Add(c);
                return new Changes(null, null);
            }

            if ((removed.Contains("\"") || removed.Contains("\'")) &&
                (
                    !removed.Contains("\"Backend.Model.TourOperator.") &&
                    !removed.Contains("[Table(\"") &&
                    !removed.Contains(".ToTable(\"") &&
                    !removed.Contains("table: \"tourop")
                ))
            {
                Changes c = new Changes(removed, added);
                CommentChanges.Add(c);
                return new Changes(null, null);
            }

            if ((added.Contains("\"") || added.Contains("\'")) &&
                (
                    !added.Contains("\"Backend.Model.TourOperator.") &&
                    !added.Contains("[Table(\"") &&
                    !added.Contains(".ToTable(\"") &&
                    !added.Contains("table: \"tourop")
                )
               )
            {
                Changes c = new Changes(removed, added);
                CommentChanges.Add(c);
                return new Changes(null, null);
            }

            bool changed = true;
            while (changed)
            {
                changed = false;
                if (removeSameIgnore(ignoreChanges, ref removed, ref added))
                {
                    changed = true;
                    continue;
                }

                bool changed1 = true;
                while (changed1)
                {
                    changed1 = false;
                    if (removeSameChars(ref removed, ref added))
                    {
                        changed1 = true;
                    }

                    removed = new string(removed.Reverse().ToArray());
                    added = new string(added.Reverse().ToArray());

                    if (removeSameChars(ref removed, ref added))
                    {
                        changed1 = true;
                    }

                    removed = new string(removed.Reverse().ToArray());
                    added = new string(added.Reverse().ToArray());
                    changed = changed | changed1;
                }

                if (changed1)
                {
                    changed = true;
                }
            }

            return new Changes(removed, added);
        }
    }
}