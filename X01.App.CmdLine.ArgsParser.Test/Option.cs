using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X01.CmdLine;

namespace X01.App.CmdLine.ArgsParser.Test;
public  class Option
{
    [CmdLineArgs(ShortName = "i", LongName = "input", HelpText = "the input files")]
    public List<string>? InputFiles { get;set;}

    public string? OutputFile { get;set;}
}
