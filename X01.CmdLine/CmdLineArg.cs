namespace X01.CmdLine;

internal class CmdLineArg
{
    public string Prefix { get;}
    public string Name { get; }
    public  string? Value { get; set; }

    protected CmdLineArg(string prefix, string name, string? value)
    {
        Prefix = prefix;
        Name = name;
        Value = value;
    }

    public override string ToString()
    {
        return $"{Prefix}{Name}{(string.IsNullOrEmpty(Value) ? string.Empty : $"={Value}")}";
    }
}

internal sealed class CmdLineShortArg : CmdLineArg
{
    public CmdLineShortArg(char name , string? value) : base("-", name.ToString(), value)
    {
    }
}

internal sealed class CmdLineShortArgs : CmdLineArg
{
    public CmdLineShortArgs(string name, string? value) : base("-", name, value)
    {
    }

    public IEnumerable<CmdLineShortArg> Args => Name.Select(x => new CmdLineShortArg(x, Value));
}

internal sealed class CmdLineLongArg : CmdLineArg
{
    public CmdLineLongArg(string name, string? value) : base("--", name, value)
    {
    }
}

internal sealed class CmdLineValueArg : CmdLineArg
{
    public CmdLineValueArg(string value) : base(string.Empty, string.Empty, value)
    {
    }
}

