// See https://aka.ms/new-console-template for more information
List<LogEntry> logs1 = new();
        foreach (var x in logFiles.Skip(1))
        {
            var logs = await MergeAsync(x, token);
            logs1.AddRange(logs);
        }

