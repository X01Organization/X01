using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using X01.CmdLine;

namespace X01.App.Json.Deserializer;
public class JsonDeserializer
{
    public async Task DeserializeAsync(IoCmdLineOption ioCmdLineOption, CancellationToken token)
    {
        string ttt = await File.ReadAllTextAsync(ioCmdLineOption.InputFileOrDirectory!);
        var ss = TryDeserialize(ttt);
        var ts = ss?.ToJsonString(new JsonSerializerOptions() { WriteIndented = true, }) ?? ttt;
        await File.WriteAllTextAsync(ioCmdLineOption.OutputFileOrDirectory!, ts);
    }

    private JsonNode? TryDeserialize(string maybeJson)
    {
        try
        {
            var jsonNode = JsonSerializer.Deserialize<JsonNode>(maybeJson);
            return TryDeserialize(jsonNode);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private JsonNode? TryDeserialize(JsonNode? jsonNode)
    {
        try
        {
            if (jsonNode is JsonObject jsonObject)
            {
                var newJsonObject = new JsonObject();
                foreach ((var key, var value) in jsonObject)
                {
                    var newValue = TryDeserialize(value) ?? value?.DeepClone();
                    newJsonObject.Add(key, newValue);
                }
                return newJsonObject;
            }

            if (jsonNode is JsonArray jsonArray)
            {
                var newJsonArray = new JsonArray();
                foreach (var value in jsonArray)
                {
                    var newValue = TryDeserialize(value) ?? value?.DeepClone();
                    newJsonArray.Add(newValue);
                }
                return newJsonArray;
            }

            if (jsonNode is JsonValue jsonValue)
            {
                var value = jsonValue.GetValue<JsonElement>();
                if (value.ValueKind == JsonValueKind.String)
                {
                    var s = jsonValue.GetValue<string>();
                    var newJsonNode = TryDeserialize(s);
                    if (null != newJsonNode)
                    {
                        return newJsonNode;
                    }
                    else
                    {
                        //s = Regex.Unescape(s);
                        var sa = Regex.Split(s, "\r\n|\r|\n");
                        if (sa.Length > 1)
                        {
                            var newJsonArray = new JsonArray();
                            foreach (var si in sa)
                            {
                                var newItemJsonNode = TryDeserialize(si) ?? JsonValue.Create(si);
                                newJsonArray.Add(newItemJsonNode);
                            }
                            return newJsonArray;
                        }

                        return JsonValue.Create(s);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("error:" + Environment.NewLine + ex.ToString());
        }

        return jsonNode?.DeepClone();
    }
}
