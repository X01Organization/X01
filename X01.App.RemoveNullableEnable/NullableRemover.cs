using System.Text;
using System.Text.RegularExpressions;

namespace X01.App.RemoveNullableEnable;

public class NullableRemover
{
    public async Task RemoveAsync(Option option, CancellationToken token)
    {
        string inputFileOrDirectory = option.InputFileOrDirectory!;
        bool nullableEnabled = option.NullableEnabled ?? true;
        Console.WriteLine("trimming " + inputFileOrDirectory + " encoding=" + Encoding.Default + ",nullableEnabled=" + nullableEnabled);
        FileAttributes attr = File.GetAttributes(inputFileOrDirectory);
        if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
        {
            await RemoveAsync(new DirectoryInfo(inputFileOrDirectory), nullableEnabled, token);
        }
        else
        {
            await RemoveAsync(new FileInfo(inputFileOrDirectory), nullableEnabled, token);
        }
    }

    private async Task RemoveAsync(FileInfo fileInfo, bool nullableEnabled, CancellationToken token)
    {
        string[] lines = File.ReadAllLines(fileInfo.FullName);
        int? indexNullableEnable = lines.Select((x, i) => new { Index = i, Line = x, })
            .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Line) &&
                                  Regex.IsMatch(x.Line, @"^\s*#nullable\s+enable\s*$"))
            ?.Index;

        int? indexNullableDisable = lines.Select((x, i) => new { Index = i, Line = x, })
            .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Line) &&
                                  Regex.IsMatch(x.Line, @"^\s*#nullable\s+disable\s*$"))
            ?.Index;
#if false
        if (null != indexNullableEnable && null == indexNullableDisable)
        {
            Console.WriteLine("-- " + fileInfo.FullName);
            var newText = string.Join(Environment.NewLine, lines.Skip(1 + indexNullableEnable!.Value));
            await File.WriteAllTextAsync(fileInfo.FullName, newText.TrimStart());
            return;
        }
        {
            Console.WriteLine("++ " + fileInfo.FullName);
            var newText = string.Join(Environment.NewLine, lines.Skip(1 + (indexNullableDisable ?? -1)).Prepend($"#nullable disable"));
            await File.WriteAllTextAsync(fileInfo.FullName, newText.TrimStart());
            return;
        }
#else
        string[] newLines = AdjustNullable(nullableEnabled, lines, indexNullableEnable, indexNullableDisable).ToArray();
#endif
        if (!newLines.SequenceEqual(lines))
        {
            string newText = string.Join(Environment.NewLine, newLines);
            await File.WriteAllTextAsync(fileInfo.FullName, newText.TrimStart());
        }
    }

    private IEnumerable<string> AdjustNullable(bool nullableEnabled, IEnumerable<string> lines, int? indexNullableEnable, int? indexNullableDisable)
    {
        if (nullableEnabled)
        {
            if (null != indexNullableEnable)
            {
                lines = RemoveNullable(lines, indexNullableEnable.Value);
            }
            else
            {
                if (null == indexNullableDisable)
                {
                    lines = AddNullableDisable(lines);
                }
            }
        }
        else
        {
            //nullable is not enabled, reverse nullable here
            if (null != indexNullableEnable)
            {
                lines = RemoveNullable(lines, indexNullableEnable.Value);
            }
            else
            {
                if (null == indexNullableDisable)
                {
                    lines = AddNullableDisable(lines);
                }
            }
        }

        return lines;
    }

    private IEnumerable<string> AddNullableDisable(IEnumerable<string> lines)
    {
        return lines.Prepend("#nullable disable");
    }

    private IEnumerable<string> RemoveNullable(IEnumerable<string> lines, int indexNullable)
    {
        return lines.Select((x, i) => new { Line = x, Index = i }).Where(x => x.Index != indexNullable).Select(x => x.Line);
    }

    private async Task RemoveAsync(DirectoryInfo directoryInfo, bool nullableEnabled, CancellationToken token)
    {
        Console.WriteLine("trimming " + directoryInfo.FullName);
        FileInfo[] files = directoryInfo.GetFiles("*.cs");
        foreach (FileInfo x in files)
        {
            await RemoveAsync(x, nullableEnabled, token);
        }

        DirectoryInfo[] directories = directoryInfo.GetDirectories();
        foreach (DirectoryInfo x in directories)
        {
            await RemoveAsync(x, nullableEnabled, token);
        }
    }
}
