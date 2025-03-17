namespace X01.CmdLine;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class CmdLineArgsAttribute : Attribute
{
    public string? ShortName { get; set; }
    public string? LongName { get; set; }
    public string? HelpText { get; set; }
    public bool Required { get; set; }
}


//TrimDistinct 1.0.0+9ee922019d6d3887ea2eb033d7a0bae356bd8918
//Copyright (C) 2023 TrimDistinct

//ERROR(S):
//  Required option 'i, input' is missing.
//  Required option 'o, output' is missing.

//  -i, --input     Required. output file fullname

//  -o, --output    Required. output file fullname

//  --help          Display this help screen.

//  --version       Display version information.

//usage: git [--version] [--help] [-C <path>] [-c <name>=<value>]
//           [--exec-path[=<path>]] [--html-path] [--man-path] [--info-path]
//           [-p | --paginate | -P | --no-pager] [--no-replace-objects] [--bare]
//           [--git-dir=<path>] [--work-tree=<path>] [--namespace=<name>]
//           [--super-prefix=<path>] [--config-env=<name>=<envvar>]
//           <command> [<args>]

