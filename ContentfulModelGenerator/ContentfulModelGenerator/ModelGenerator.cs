using ContentfulModelGenerator.Dto;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace ContentfulModelGenerator
{
    public class ModelGenerator
    {
        public async Task GenerateAsync(CancellationToken token = default)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AUTH);
            var response = await httpClient.GetAsync(URL);
            //var test = await response.Content.ReadAsStringAsync();
            var cfModel = await response.Content.ReadFromJsonAsync<CfModel>();
            await GenerateAsync(cfModel, token);
        }

        private async Task GenerateAsync(CfModel cfModel, CancellationToken token)
        {
            var dir = new DirectoryInfo("c:/workroot/Contentful/");
            if (dir.Exists)
            {
                dir.Delete(true);
            }

            dir.Create();

            var typeIdSb = new StringBuilder();

            typeIdSb.AppendLine("namespace Nirvana.Dto.Contentful;");
            typeIdSb.AppendLine();
            typeIdSb.AppendLine($"public class ContentfulTypeIds");
            typeIdSb.AppendLine("{");


            foreach (var x in cfModel.Items.OrderBy(x => x.Sys.Id))
            {
                if (x.Sys.Id.Contains("test", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var sb1 = new StringBuilder();
                await GenerateModelAsync(sb1, x, token);
                var content = sb1.ToString();

                var sb = new StringBuilder();
                if (content.Contains("Converter))]"))
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

                sb.Append(content);
                sb.AppendLine("}");

                File.WriteAllText($"{dir.FullName}/Contentful" + GetFieldName(x.Sys.Id) + ".cs", sb.ToString().Trim());

                typeIdSb.AppendLine($"    public const string {GetFieldName(x.Sys.Id)} = \"{x.Sys.Id}\";");
            }

            typeIdSb.AppendLine("}");
            File.WriteAllText($"{dir.FullName}/ContentfulTypeIds.cs", typeIdSb.ToString().Trim());

            return;

            foreach (var x in cfModel.Items)
            {
                var sb = new StringBuilder();
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
            foreach (var x in cmpfieldNames)
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
            var allfields = cfModelItem.Fields.Where(x => true != x.Disabled).ToArray();
            int i = 0;
            foreach (var x in allfields.OrderBy(x =>
            {
                var fieldName = GetFieldName(x.Id);
                int order = 0;
                var fieldNames = new[]
                {
                    "InternalName", "title", "Description", "productLine", "headline",
                }.SelectMany(x => new[] {x, "sub" + x,}).ToArray();

                if (GetOrder(ref order, fieldName, fieldNames))
                {
                    return string.Format("{0:0000}", order);
                }

                if (fieldName.Contains("debug", StringComparison.OrdinalIgnoreCase))
                {
                    return "zzzzzzzz" + fieldName;
                }

                var fieldTypeName = GetFieldTypeName(x);
                if (fieldTypeName == "Link")
                {
                    return "zzzz0000" + fieldName;
                }

                return fieldName;
            }))
            {
                ++i;
                var fieldTypeName = GetFieldTypeName(x);
                if ("Link" == fieldTypeName)
                {
                    var linkcontenttypes =
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

            var linkContentTypes = field.Items
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
                var destinationTypeIds = new[]
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
            foreach (var x in cfModelItem.Fields.Where(x => true != x.Disabled).OrderBy(x =>
            {
                var fieldName = GetFieldName(x.Id);
                int order = 0;
                var fieldNames = new[]
                {
                    "InternalName", "title", "Description", "productLine", "headline",
                }.SelectMany(x => new[] {x, "sub" + x,}).ToArray();

                if (GetOrder(ref order, fieldName, fieldNames))
                {
                    return string.Format("{0:0000}", order);
                }

                if (fieldName.Contains("debug", StringComparison.OrdinalIgnoreCase))
                {
                    return "zzzzzzzz" + fieldName;
                }

                var fieldTypeName = GetFieldTypeName(x);
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