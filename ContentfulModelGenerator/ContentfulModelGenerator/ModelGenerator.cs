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
            var cfModel = await response.Content.ReadFromJsonAsync<CfModel>();
            await GenerateAsync(cfModel, token);
        }

        private async Task GenerateAsync(CfModel cfModel, CancellationToken token)
        {
            foreach (var x in cfModel.Items)
            {
                var sb = new StringBuilder();
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine();
                sb.AppendLine("namespace Nirvana.Dto.Contentful;");
                sb.AppendLine();
                sb.AppendLine($"public sealed class Contentful{GetFieldName(x.Sys.Id)}");
                sb.AppendLine("{");

                await GenerateModelAsync(sb, x, token);

                sb.AppendLine("}");

                File.WriteAllText("c:/workroot/tmp/Contentful" + GetFieldName(x.Sys.Id) + ".cs", sb.ToString());
            }

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
                var fieldtypename = GetFieldTypeName(x);
                if ("Link" == fieldtypename)
                {
                    var linkcontenttypes =
                        x.Validations.SelectMany(x => x!.LinkContentType).Where(x => null != x).ToArray();
                    if (1 == linkcontenttypes.Length && !x.Id.Contains("test", StringComparison.OrdinalIgnoreCase))
                    {
                        fieldtypename = "Contentful" + GetFieldName(linkcontenttypes[0]);
                    }
                    else
                    {
                        int io = 0;
                    }
                }

                if ("List<Link>" == fieldtypename)
                {
                    var linkcontenttypes =
                        x.Items.Validations.SelectMany(x => x!.LinkContentType).Where(x => null != x).ToArray();
                    if (1 == linkcontenttypes.Length && !x.Id.Contains("test", StringComparison.OrdinalIgnoreCase))
                    {
                        fieldtypename = "List<Contentful" + GetFieldName(linkcontenttypes[0]) + ">";
                    }
                    else
                    {
                        if (1 < linkcontenttypes.Length && !x.Id.Contains("test", StringComparison.OrdinalIgnoreCase))
                        {
                            fieldtypename = "List<I" + GetFieldName(x.Id) + ">";
                            sb.AppendLine($"    public {fieldtypename}? {GetFieldName(x.Id)} "
                                        + "{ get; set; }");
                            sb.AppendLine();
                            foreach (var y in linkcontenttypes)
                            {
                                sb.AppendLine($"    public List<Contentful{GetFieldName(y)}>? {GetFieldName(y)}s "
                                            + "=> " + GetFieldName(x.Id) + $".Where(x=> x is Contentful{GetFieldName(y)}).ToList();");
                                sb.AppendLine();
                            }

                            return;
                        }
                    }
                }

                sb.AppendLine($"    public {fieldtypename}? {GetFieldName(x.Id)} "
                            + "{ get; set; }");
                sb.AppendLine();
            }
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
                case "Link":
                {
                    return "Link";
                }
                case "Number":
                {
                    return "double";
                }
                case "Object":
                {
                    return "List<ContentfulImage>";
                }
                case "RichText":
                {
                    return "string";
                }
                case "Symbol":
                {
                    return "string";
                }
                case "Text":
                {
                    return "string";
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