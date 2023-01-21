using System.Text.Json;
using System.Text.Json.Nodes;

var base64JsonString =  args[0];
if (!string.IsNullOrEmpty(base64JsonString))
{
    byte[] jsonBytes = null;
    try
    {
        jsonBytes = Convert.FromBase64String(base64JsonString);
    }
    catch (FormatException)
    {
        base64JsonString = base64JsonString.Replace('-', '+')
                                           .Replace('_', '/');
        base64JsonString =
            base64JsonString.PadRight(base64JsonString.Length + (base64JsonString.Length * 3) % 4, '=');
        jsonBytes = Convert.FromBase64String(base64JsonString);
    }

    var base64JsonParameterJsonObject = JsonSerializer.Deserialize<JsonObject>(jsonBytes);
}