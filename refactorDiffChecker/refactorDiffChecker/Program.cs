﻿// See https://aka.ms/new-console-template for more information

using refactorDiffChecker;
using System.Text.RegularExpressions;

List<Changes> ignorChanges = new List<Changes>()
{
    //new Changes("SERVICE", "ACTIVITY"),
    //new Changes("Service", "Activity"),
    //new Changes("service", "activity"),
    //new Changes("servic", "activiti"),
    //new Changes("Servic", "Activiti"),
    //new Changes("SHIPPING", "TRANSPORT"),
    //new Changes("Shipping", "Transport"),
    //new Changes("shipping", "transport"),
    //new Changes("shippings", "transports"),
    new Changes("ArrivalDate", "ArrivalTime"),
    new Changes("DepartureDate", "DepartureTime"),
};
var lines = File.ReadAllLines("C:\\Users\\zxiang\\project\\1.diff");

using (var fs = new FileStream("c:\\temp\\result.diff", FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
{
    using (var sw = new StreamWriter(fs))
    {
        var parser = new Parser();
        var changedFiles = parser.GetChangedFiles(lines);
        foreach (var fileChanges in changedFiles)
        {
            var changes = parser.GetChanges(fileChanges);
            if (changes.FilenameChanges.Removed.EndsWith("ev/null")
             && changes.FilenameChanges.Added.StartsWith("Backend/Backend.DB/Migrations/TourOperator/"))
            {
                continue;
            }

            bool hasChanges = false;
            if (changes.FilenameChanges.Removed != changes.FilenameChanges.Added)
            {
                var ffc = parser.GetChanges(ignorChanges, changes.FilenameChanges.Removed,
                    changes.FilenameChanges.Added);
                if (!ffc.NoChange)
                {
                    hasChanges = true;
                    System.Diagnostics.Debug.WriteLine("-" + ffc.Removed);
                    System.Diagnostics.Debug.WriteLine("+" + ffc.Added);
                    sw.WriteLine("-" + ffc.Removed);
                    sw.WriteLine("+" + ffc.Added);
                }
            }

            foreach (var change in changes.Changes)
            {
                string[] removedLines =
                    null == change.Removed ? Array.Empty<string>() : Regex.Split(change.Removed, "\r\n|\r|\n");
                string[] addedLines = null == change.Added
                    ? Array.Empty<string>()
                    : Regex.Split(change.Added, "\r\n|\r|\n");
                if (removedLines.Length == addedLines.Length)
                {
                    int i = 0;
                    while (i < removedLines.Length)
                    {
                        var c = parser.GetChanges(ignorChanges, removedLines[i], addedLines[i]);
                        if (!c.NoChange)
                        {
                            hasChanges = true;
                            System.Diagnostics.Debug.WriteLine("-" + c.Removed);
                            System.Diagnostics.Debug.WriteLine("+" + c.Added);
                            System.Diagnostics.Debug.WriteLine(removedLines[i] + "==>" + addedLines[i]);
                            sw.WriteLine("-" + c.Removed);
                            sw.WriteLine("+" + c.Added);
                        }

                        ++i;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(string.Join(Environment.NewLine,
                        removedLines.Select(x => '-' + x)));
                    System.Diagnostics.Debug.WriteLine(
                        string.Join(Environment.NewLine, addedLines.Select(x => '+' + x)));
                    sw.WriteLine(string.Join(Environment.NewLine,
                        removedLines.Select(x => '-' + x)));
                    sw.WriteLine(string.Join(Environment.NewLine, addedLines.Select(x => '+' + x)));
                }
            }

            if (hasChanges)
            {
                System.Diagnostics.Debug.WriteLine(" file changed: " + changes.FilenameChanges.Removed + "==>"
                                                 + changes.FilenameChanges.Added);
            }
        }

        foreach (var c in parser.CommentChanges)
        {
            System.Diagnostics.Debug.WriteLine("{\"" + c.Removed.Replace("\'", "\\\'").Replace("\"", "\\\"") + "\",\"" +
                                               c.Added.Replace("\'", "\\\'").Replace("\"", "\\\"") + "\"}");

            sw.WriteLine("-" + c.Removed);
            sw.WriteLine("+" + c.Added);
        }
    }
}