using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace X01.App.Json.Deserializer;
public class JsonDeserializer
{
    public async Task DeserializeAsync(Option option, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(option.InputFileOrDirectory))
        {
            throw new ArgumentException("No input file or directory specified");
        }

        string inputFileOrDirectory = option.InputFileOrDirectory;
        FileAttributes attr = File.GetAttributes(inputFileOrDirectory);
        if (attr.HasFlag(FileAttributes.Directory))
        {
            DirectoryInfo directoryInfo = new(inputFileOrDirectory);
            FileInfo[] infputFiles = directoryInfo.GetFiles("*", new EnumerationOptions()
            {
                MaxRecursionDepth = 10000,
                RecurseSubdirectories = true,
                ReturnSpecialDirectories = false,
            });
            foreach (FileInfo x in infputFiles)
            {
                await DeserializeAsync(x,option.PathRegex, token);
            }
        }
        else
        {
                await DeserializeAsync(new FileInfo(option.InputFileOrDirectory),option.PathRegex, token);
        }
    }
    private async Task DeserializeAsync(FileInfo jsonFileInfo,string? pathRegex, CancellationToken token)
    {
        string ttt = await File.ReadAllTextAsync(jsonFileInfo.FullName, token);
        JsonNode? rootNode = TryDeserialize(ttt);
        PrintMatchedValues(rootNode,pathRegex);
    }

    private void PrintMatchedValues(JsonNode? node, string? pathRegex)
    {
        if (null == node)
        {
            return;
        }

        if (node is JsonArray jsonArray)
        {
            foreach (JsonNode? x in jsonArray)
            {
                PrintMatchedValues(x, pathRegex);
            }
            return;
        }

        if (node is JsonObject jsonObject)
        {
            foreach (KeyValuePair<string, JsonNode?> x in jsonObject)
            {
                PrintMatchedValues(x.Value, pathRegex);
            }
            return;
        }

        if (null == pathRegex || Regex.IsMatch(node.GetPath(),pathRegex ))
        {
            Console.WriteLine($"Found=> { node.ToJsonString()}");
        }
    }

    private JsonNode? TryDeserialize(string maybeJson)
    {
        try
        {
            JsonNode? jsonNode = JsonSerializer.Deserialize<JsonNode>(maybeJson);
            return jsonNode;
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
