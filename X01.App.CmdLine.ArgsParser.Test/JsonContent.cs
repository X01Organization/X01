using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X01.App.CmdLine.ArgsParser.Test;

public class JsonContent : StringContent
{
    public JsonContent(string json) : this(json, Encoding.UTF8)
    {
    }

    public JsonContent(string json, Encoding encoding) : base(json, encoding, "application/json")
    {
    }

    ~JsonContent(){ 
        Dispose(false);
    }
    protected override void Dispose(bool disposing)
    {
        Console.WriteLine($"Dispose({disposing})");

        base.Dispose(disposing);
    }
}
