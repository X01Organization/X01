using X01.App.TrimDistinct;
using X01.CmdLine;
using System.Linq;

Option option = new CmdLineArgsParser().Parse<Option>(args);

string output = option.Output ?? option.Input;

string[] lines = File.ReadAllLines(option.Input!)
                   .Select(line => line.Trim())
                   .Where(line => !string.IsNullOrEmpty(line))
                   .Distinct()
                   .OrderBy(line => line).ToArray();

File.WriteAllLines(output, lines);
