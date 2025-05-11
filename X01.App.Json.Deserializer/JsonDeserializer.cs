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
        JsonNode? ss = TryDeserialize(ttt);
        string ts = ss?.ToJsonString(new JsonSerializerOptions() { WriteIndented = true, }) ?? ttt;
        await File.WriteAllTextAsync(ioCmdLineOption.OutputFileOrDirectory!, ts);
    }

    private JsonNode? TryDeserialize(string maybeJson)
    {
        try
        {
            JsonNode? jsonNode = JsonSerializer.Deserialize<JsonNode>(maybeJson);
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
                JsonObject newJsonObject = new();
                foreach ((string? key, JsonNode? value) in jsonObject)
                {
                    JsonNode? newValue = TryDeserialize(value) ?? value?.DeepClone();
                    newJsonObject.Add(key, newValue);
                }
                return newJsonObject;
            }

            if (jsonNode is JsonArray jsonArray)
            {
                JsonArray newJsonArray = new();
                foreach (JsonNode? value in jsonArray)
                {
                    JsonNode? newValue = TryDeserialize(value) ?? value?.DeepClone();
                    newJsonArray.Add(newValue);
                }
                return newJsonArray;
            }

            if (jsonNode is JsonValue jsonValue)
            {
                JsonElement value = jsonValue.GetValue<JsonElement>();
                if (value.ValueKind == JsonValueKind.String)
                {
                    string s = jsonValue.GetValue<string>();
                    JsonNode? newJsonNode = TryDeserialize(s);
                    if (null != newJsonNode)
                    {
                        return newJsonNode;
                    }
                    else
                    {
                        //s = Regex.Unescape(s);
                        string[] sa = Regex.Split(s, "\r\n|\r|\n");
                        if (sa.Length > 1)
                        {
                            JsonArray newJsonArray = new();
                            foreach (string si in sa)
                            {
                                JsonNode newItemJsonNode = TryDeserialize(si) ?? JsonValue.Create(si);
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
