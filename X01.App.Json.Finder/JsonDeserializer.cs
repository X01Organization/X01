using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using X01.CmdLine;

namespace X01.App.Json.Deserializer;
public class JsonDeserializer
{
    public async Task DeserializeAsync(Option option, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(option.InputFileOrDirectory))
        {
            throw new ArgumentException("No input file or directory specified");
        }

        var inputFileOrDirectory = option.InputFileOrDirectory;
        FileAttributes attr = File.GetAttributes(inputFileOrDirectory);
        if (attr.HasFlag(FileAttributes.Directory))
        {
            var directoryInfo = new DirectoryInfo(inputFileOrDirectory);
            var infputFiles = directoryInfo.GetFiles("*", new EnumerationOptions()
            {
                MaxRecursionDepth = 10000,
                RecurseSubdirectories = true,
                ReturnSpecialDirectories = false,
            });
            foreach (var x in infputFiles)
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
        var rootNode = TryDeserialize(ttt);
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
            foreach (var x in jsonArray)
            {
                PrintMatchedValues(x, pathRegex);
            }
            return;
        }

        if (node is JsonObject jsonObject)
        {
            foreach (var x in jsonObject)
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
            var jsonNode = JsonSerializer.Deserialize<JsonNode>(maybeJson);
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
