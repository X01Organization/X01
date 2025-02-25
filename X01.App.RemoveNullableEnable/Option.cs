using X01.CmdLine;

namespace X01.App.RemoveNullableEnable;

public class Option
{
    [CmdLineArgs(ShortName = "i", LongName = "input", HelpText = "the input directory")]
    public string? InputFileOrDirectory { get; set; }

    [CmdLineArgs(ShortName = "n", LongName = "nullable", HelpText = "0: nullable disabled, 1: nullable enabled")]
    public bool? NullableEnabled { get; set; }
}
