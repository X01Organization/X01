// See https://aka.ms/new-console-template for more information
        var logs1 = new List<LogEntry>();
        foreach (var x in logFiles.Skip(1))
        {
            var logs = await MergeAsync(x, token);
            logs1.AddRange(logs);
        }

