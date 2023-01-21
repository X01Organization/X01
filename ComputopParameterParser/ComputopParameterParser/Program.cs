// See https://aka.ms/new-console-template for more information

using ComputopParameterParser.Data;
using ComputopParameterParser.Definition;
using ComputopParameterParser.Util;

var lines = File.ReadAllLines(@"C:\Temp\1");
var lineInfos = lines.Select(x => x.Split('\t')).ToArray();
var dijsis = lineInfos.Select(info =>
{
    if (4 != info.Length)
    {
        throw new Exception("Not valid line: " + string.Join('\t', info));
    }
    var di = new DefinitioinInfo(info[0], info[1], info[2], info[3]
        .Replace(" " + ((char)160).ToString() + " ", " ")
        .Replace(((char)160).ToString() + " ", " ")
        .Replace(" " + ((char)160).ToString(), " ")
        .Replace(((char)160).ToString(), " ")
                                                                   );
    var jsi = DataFormats.GetJsonSchemaInfo(di);
    var abjsi = Abbreviations.GetJsonSchemaInfo(di);
    jsi.IsRequired = abjsi.IsRequired;
    return (di, jsi);
}).ToArray();
var specialNames = new Dictionary<Type,string>()
{
    { typeof(long), "long" },
    { typeof(int), "int" },
    { typeof(bool), "bool" },
};
File.WriteAllText(@"C:\Temp\1.cs", string.Join(Environment.NewLine, dijsis.Select(dijsi =>
{
    var di = dijsi.di;
    var jsi = dijsi.jsi;
    var comments = new List<string> {
        "<summary>",
        di.Format + "   " + di.Condition,
        "</summary>",
    };
    
    var codes = new List<string>(){
        "[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]",
        $"[JsonPropertyName(\"{di.Name}\")]",
        "public " + (specialNames.ContainsKey(jsi.Type)?specialNames[jsi.Type]:jsi.Type.Name.ToLower()) + " " + char.ToUpper( di.Name[0])  + di.Name.Substring(1)+ " { get; set; }",
    };
    comments.InsertRange(2, StringUtil.Split(di.Description.Replace("<", "⟨").Replace(">", "⟩"), 100).Select(x => x.Trim()));
    return string.Join(Environment.NewLine, comments.Select(x => "    /// " + x).Concat(codes.Select(x => "    " + x)));
})));
