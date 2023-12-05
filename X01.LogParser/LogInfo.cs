namespace X01.LogParser;
public class LogInfo
{
    public DateTime? Time { get; set; }
    public string? Thread { get; set; }
    public string? Type { get; set; }
    public string? SubType { get; set; }
    public string? Class { get; set; }
    public List<string> Lines { get; set; } = new List<string>();
}
