using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using ContentfulModelGenerator.Dto;

namespace ContentfulModelGenerator
{
    public class ModelGenerator
    {
        private const string URL = "https://api.contentful.com/spaces/iipo05hi434x/environments/test/content_types";

        public async Task GenerateAsync(CancellationToken token = default)
        {
            using HttpClient httpClient = new();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AUTH);
            HttpResponseMessage response = await httpClient.GetAsync(URL);
            string test = await response.Content.ReadAsStringAsync();

            await File.WriteAllTextAsync("c:/workroot/contentfulModel.json", test, token);

            JsonNode contentfulModel = JsonSerializer.Deserialize<JsonNode>(test)!;

            WriteObjectType(contentfulModel);

            CfModel? cfModel = await response.Content.ReadFromJsonAsync<CfModel>();
            await GenerateAsync(cfModel, token);
        }

        private void WriteObjectType(JsonNode jsonNode)
        {
            Dictionary<string, List<string>> testssss = new();
            JsonNode? items = jsonNode.AsObject()["items"];
            foreach (JsonNode? z in items!.AsArray())
            {
                JsonObject x = z!.AsObject();
                JsonArray fileds = x["fields"]!.AsArray();
                foreach (JsonNode? y in fileds)
                {
                    JsonObject property = y!.AsObject();
                    JsonNode? type = property["type"];
                    if (type!.GetValue<string>() == "Object")
                    {
                        string imagePropertyId = property["id"]!.GetValue<string>();
                        string entryTypeId = x["sys"]!.AsObject()["id"]!.GetValue<string>();
                        if(!  testssss.TryGetValue(entryTypeId, out List<string>? lst)){ 
lst = new List<string>();
testssss.Add( entryTypeId, lst );
                        }

                        lst.Add(imagePropertyId );
                        Debug.WriteLine($"{entryTypeId}==>{imagePropertyId}");
                    }
                }
            }

            string test11j = string.Join('\n', testssss.Select(x=>x.Key + "\n\t" + string.Join("\n\t" ,x.Value.Select(y => y))).OrderBy(x=> x));
            string test = string.Join('\n', testssss.SelectMany(x=> x.Value).Select(x=> $"    public Dictionary<string, ContentfulImage[] >? {char.ToUpper( x[0])}{x.Substring(1)}"+" { get; set; }"));
            int a3 = 0;
            if (a3 == 0)
            {
                return;
            }
        }

        private async Task GenerateAsync(CfModel cfModel, CancellationToken token)
        {
            DirectoryInfo dir = new("c:/workroot/Contentful/");
            if (dir.Exists)
            {
                dir.Delete(true);
            }

            dir.Create();

            List<string> allLocalizedFields = new();
            StringBuilder typeIdSb = new();

            typeIdSb.AppendLine("using System.Collections.ObjectModel;");
            typeIdSb.AppendLine();
            typeIdSb.AppendLine("namespace Nirvana.Dto.Contentful;");
            typeIdSb.AppendLine();
            typeIdSb.AppendLine($"public class ContentfulTypeIds");
            typeIdSb.AppendLine("{");
            typeIdSb.AppendLine("    public const string ContentTypeId = \"contentTypeId\";");
            typeIdSb.AppendLine();


            foreach (CfModelItems? x in cfModel.Items.OrderBy(x => x.Sys.Id))
            {
                if (x.Sys.Id.Contains("test", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                bool isdestination = new[] { "Continent", "Country", "Region", "City", "Highlight", }
                .Contains(GetFieldName(x.Sys.Id));

                StringBuilder sb1 = new();
                await GenerateModelAsync(sb1, x, token);
                string content = sb1.ToString();

                StringBuilder sb = new();
                if (content.Contains("Json") || isdestination)
                {
                    sb.AppendLine("using System.Text.Json.Serialization;");
                    sb.AppendLine();
                }

                //sb.AppendLine("using System.Collections.Generic;");
                //sb.AppendLine();
                sb.AppendLine("namespace Nirvana.Dto.Contentful;");
                sb.AppendLine();
                sb.AppendLine($"public sealed class Contentful{GetFieldName(x.Sys.Id)}");
                sb.AppendLine("{");

                sb.AppendLine($"    public const string ContentTypeId = ContentfulTypeIds.{GetFieldName(x.Sys.Id)};");
                sb.AppendLine();

                if (isdestination)
                {
                    sb.AppendLine("    [JsonIgnore]");
                    sb.AppendLine("    public int? DestinationOrder { get; set; }");
                    sb.AppendLine();
                }

                sb.Append(content);
                if ("TextModule" == GetFieldName(x.Sys.Id))
                {

                    sb.AppendLine();
                    sb.AppendLine(@"    public override string? ToString()
    {
        return Text;
    }");
                }
                sb.AppendLine("}");

                File.WriteAllText($"{dir.FullName}/Contentful" + GetFieldName(x.Sys.Id) + ".cs", sb.ToString().Trim());

                typeIdSb.AppendLine($"    public const string {GetFieldName(x.Sys.Id)} = \"{x.Sys.Id}\";");

                string[] localizedFields = x.Fields.Where(y => true == y.Localized)
                    .Select(y => "$\"{" + GetFieldName(x.Sys.Id) + "}." + y.Id + "\"")
                    .ToArray();
                allLocalizedFields.AddRange(localizedFields);
            }

            typeIdSb.AppendLine();
            allLocalizedFields = allLocalizedFields.OrderBy(x => x).
                Select(x => $"        {x},")
                .ToList();
            typeIdSb.AppendLine("    public static readonly ReadOnlyCollection<string> LocalizedFields = new List<string>");
            typeIdSb.AppendLine("    {");
            typeIdSb.AppendLine(string.Join(Environment.NewLine, allLocalizedFields));
            typeIdSb.AppendLine("    }.AsReadOnly();");

            typeIdSb.AppendLine("}");
            File.WriteAllText($"{dir.FullName}/ContentfulTypeIds.cs", typeIdSb.ToString().Trim());

            //allLocalizedFields =    allLocalizedFields.OrderBy(x=> x).ToList();
            //var sb5 = new StringBuilder();
            //File.WriteAllText($"{dir.FullName}/ContentfulLocalizedFields.cs", sb5.ToString().Trim());
            return;

            foreach (CfModelItems x in cfModel.Items)
            {
                StringBuilder sb = new();
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using System.Text.Json.Serialization;");
                sb.AppendLine("using API.Contentful.Core;");
                sb.AppendLine();
                sb.AppendLine("namespace API.Contentful.ContentTypes;");
                sb.AppendLine();
                sb.AppendLine($"public sealed class {GetFieldName(x.Sys.Id)} : IContentType");
                sb.AppendLine("{");

                sb.AppendLine("    [JsonIgnore]");
                sb.AppendLine($"    public string ContentTypeId => \"{x.Sys.Id}\";");
                sb.AppendLine();

                await GenerateAsync(sb, x, token);

                sb.AppendLine("}");

                File.WriteAllText("c:/workroot/tmp/" + GetFieldName(x.Sys.Id) + ".cs", sb.ToString());
            }
        }

        private bool GetOrder(ref int order, string fieldName, string[] cmpfieldNames)
        {
            foreach (string x in cmpfieldNames)
            {
                if (GetOrder1(ref order, fieldName, x))
                {
                    return true;
                }
            }

            return false;
        }

        private bool GetOrder1(ref int order, string fieldName, string cmpfieldName)
        {
            ++order;
            if (fieldName.Equals(cmpfieldName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private async Task GenerateModelAsync(StringBuilder sb, CfModelItems cfModelItem, CancellationToken token)
        {
            await Task.Delay(1);
            CfModelItemsFields[] allfields = cfModelItem.Fields.Where(x => true != x.Disabled).ToArray();
            int i = 0;
            foreach (CfModelItemsFields? x in allfields.OrderBy(x =>
            {
                string fieldName = GetFieldName(x.Id);
                int order = 0;
                string[] fieldNames = new[]
                {
                    "InternalName", "title", "Description", "productLine", "headline",
                }.SelectMany(x => new[] { x, "sub" + x, }).ToArray();

                if (GetOrder(ref order, fieldName, fieldNames))
                {
                    return string.Format("{0:0000}", order);
                }

                if (fieldName.Contains("debug", StringComparison.OrdinalIgnoreCase))
                {
                    return "zzzzzzzz" + fieldName;
                }

                string fieldTypeName = GetFieldTypeName(x);
                if (fieldTypeName == "Link")
                {
                    return "zzzz0000" + fieldName;
                }

                return fieldName;
            }))
            {
                ++i;
                string fieldTypeName = GetFieldTypeName(x);
                if ("Link" == fieldTypeName)
                {
                    string[] linkcontenttypes =
                        x.Validations.SelectMany(x => x!.LinkContentType).Where(x => null != x).ToArray();
                    if (1 == linkcontenttypes.Length && !x.Id.Contains("test", StringComparison.OrdinalIgnoreCase))
                    {
                        fieldTypeName = "Contentful" + GetFieldName(linkcontenttypes[0]);
                    }
                    else
                    {
                        if (x.LinkType == "Asset")
                        {
                            fieldTypeName = "ContentfulAsset";
                        }
                        else
                        {
                            throw new Exception("bug");
                        }
                    }
                }

                if ("List<Link>" == fieldTypeName)
                {
                    fieldTypeName = GetLinkListTypeName(x);
                    if (null == fieldTypeName)
                    {
                        throw new Exception();
                    }
                }

                sb.AppendLine("    /// <summary>");
                sb.AppendLine($"    /// Localized={x.Localized}");
                sb.AppendLine($"    /// Required={x.Required}");
                sb.AppendLine($"    /// Omitted={x.Omitted}");
                sb.AppendLine("    /// </summary>");

                if (x.Type == "RichText" && fieldTypeName == "ContentfulRichText")
                {
                    sb.AppendLine("    [JsonConverter(typeof(ContentfulRichTextConverter))]");
                }

                if (fieldTypeName == "ContentfulDestinations")
                {
                    sb.AppendLine("    [JsonConverter(typeof(ContentfulDestinationsConverter))]");
                }

                if (fieldTypeName == "ContentfulTeaser")
                {
                    sb.AppendLine("    [JsonConverter(typeof(ContentfulRichTextTeaserConverter))]");
                }

                if (true == x.Required)
                {
                    sb.AppendLine("    [JsonRequired]");
                }

                sb.AppendLine($"    public {fieldTypeName}? {GetFieldName(x.Id)} "
                            + "{ get; set; }");
                if (i != allfields.Length)
                {
                    sb.AppendLine();
                }
            }
        }

        private string GetLinkListTypeName(CfModelItemsFields field)
        {
            if (field.Id.Contains("test", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            string[] linkContentTypes = field.Items
                                        .Validations
                                        .SelectMany(x => x!.LinkContentType)
                                        .Where(x => null != x)
                                        .ToArray();

            if (1 == linkContentTypes.Length)
            {
                return "List<Contentful" + GetFieldName(linkContentTypes[0]) + ">";
            }

            if (1 < linkContentTypes.Length)
            {
                string[] destinationTypeIds = new[]
                {
                    "continent", "country", "highlight", "region", "city",
                };

                if (linkContentTypes.Intersect(destinationTypeIds).Any())
                {
                    return "ContentfulDestinations";
                }
            }

            if (field.Id == "reviews")
            {
                return "List<ContentfulReview>";
            }

            if (field.Id == "tourOpIncluded")
            {
                return "List<ContentfulIncludedItems>";
            }

            return null;
        }

        private async Task GenerateAsync(StringBuilder sb, CfModelItems cfModelItem, CancellationToken token)
        {
            await Task.Delay(1);
            foreach (CfModelItemsFields? x in cfModelItem.Fields.Where(x => true != x.Disabled).OrderBy(x =>
            {
                string fieldName = GetFieldName(x.Id);
                int order = 0;
                string[] fieldNames = new[]
                {
                    "InternalName", "title", "Description", "productLine", "headline",
                }.SelectMany(x => new[] { x, "sub" + x, }).ToArray();

                if (GetOrder(ref order, fieldName, fieldNames))
                {
                    return string.Format("{0:0000}", order);
                }

                if (fieldName.Contains("debug", StringComparison.OrdinalIgnoreCase))
                {
                    return "zzzzzzzz" + fieldName;
                }

                string fieldTypeName = GetFieldTypeName(x);
                if (fieldTypeName == "Link")
                {
                    return "zzzz0000" + fieldName;
                }

                return fieldName;
            }))
            {
                sb.AppendLine("    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]");
                sb.AppendLine($"    public Dictionary<string, {GetFieldTypeName(x)}>? {GetFieldName(x.Id)} "
                            + "{ get; set; }");
                sb.AppendLine();
            }
        }

        private string GetFieldTypeName(string fieldType)
        {
            switch (fieldType)
            {
                case "Boolean":
                    {
                        return "bool";
                    }
                case "Integer":
                    {
                        return "int";
                    }
                case "Number":
                    {
                        return "double";
                    }
                case "Object":
                    {
                        return "List<ContentfulImage>";
                    }
                case "Symbol":
                    {
                        return "string";
                    }
                case "Text":
                    {
                        return "string";
                    }
                case "RichText":
                    {
                        return "ContentfulRichText";
                    }
                case "Link":
                    {
                        return "Link";
                    }
            }

            throw new Exception(fieldType);
        }

        private string GetFieldTypeName(CfModelItemsFields field)
        {
            if ("Array" == field.Type)
            {
                return $"List<{GetFieldTypeName(field.Items.Type)}>";
            }
            else
            {
                if ("Integer" == field.Type && (field.Id == "typo3Uid" || "tourOpTripId" == field.Id))
                {
                    return "uint";
                }

                if ("RichText" == field.Type && field.Id.Contains("Teaser", StringComparison.OrdinalIgnoreCase))
                {
                    return "ContentfulTeaser";
                }

                return GetFieldTypeName(field.Type);
            }
        }

        private string GetFieldName(string fieldName)
        {
            if (char.IsLower(fieldName[0]))
            {
                return char.ToUpperInvariant(fieldName[0]) + fieldName.Substring(1);
            }

            return fieldName;
        }
    }
}