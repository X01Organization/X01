using ComputopParameterParser.Data;

namespace ComputopParameterParser.Definition
{
    /// <summary>
    /// @ref https://developer.computop.com/pages/viewpage.action?pageId=26247171
    /// @ref https://json-schema.org/understanding-json-schema/reference/regular_expressions.html
    /// </summary>
    public sealed class DataFormats
    {
        private static readonly Dictionary<string, JsonSchemaInfo> _dict = new Dictionary<string, JsonSchemaInfo>()
        {
            { "", new JsonSchemaInfo(){Type  = typeof(string) } },
            { "a", new JsonSchemaInfo(){Type = typeof(string), Pattern = "[a-z]" } },
            { "as", new JsonSchemaInfo(){Type = typeof(string), Pattern = "." } },
            { "a,s", new JsonSchemaInfo(){Type = typeof(string), Pattern = "." } },
            { "n", new JsonSchemaInfo(){Type = typeof(long), Pattern = "[0-9]" } },
            { "an", new JsonSchemaInfo(){Type = typeof(string), Pattern = "[a-z0-9]" } },
            { "ans", new JsonSchemaInfo(){Type = typeof(string), Pattern = "." } },
            { "ns", new JsonSchemaInfo(){Type = typeof(string), Pattern = "." } },
            { "bool", new JsonSchemaInfo(){Type = typeof(bool) } },
            { "boolean", new JsonSchemaInfo(){Type = typeof(bool) } },
            { "enum", new JsonSchemaInfo() },
            { "a2 (enum)", new JsonSchemaInfo() {Type = typeof(string) } },
            { "JSON", new JsonSchemaInfo() {Type = typeof(string) } },
            { "dttm", new JsonSchemaInfo(){Type = typeof(string), Pattern = "YYYY-MM-DDThh:mm:ss" } },
            { "string", new JsonSchemaInfo(){Type = typeof(string)  } },
            { "integer", new JsonSchemaInfo(){Type = typeof(int)  } },
            { "object", new JsonSchemaInfo(){Type = typeof(string)  } },
        };
        public static JsonSchemaInfo GetJsonSchemaInfo(DefinitioinInfo di)
        {
            string fmt = di.Format;
            int len = FindDigitLength(fmt);
            JsonSchemaInfo jsi;
            if (len > 0)
            {
                string fmt1 = fmt.Substring(0, fmt.Length - len);
                int limit = int.Parse(fmt.Substring(fmt.Length - len, len));
                fmt = fmt1;
                if (fmt.EndsWith(".."))
                {
                    fmt = fmt.Substring(0, fmt.Length - 2);
                    jsi = _dict[fmt];
                    jsi.Pattern = "{0," + limit + "}";
                }
                else
                {
                    jsi = _dict[fmt];
                    jsi.Pattern = "{" + limit + "}";
                }
            }
            else
            {
                jsi = _dict[fmt];
            }
            return jsi;
        }
        private static int FindDigitLength(string s)
        {
            int len = 0;
            int i = s.Length - 1;
            while (0 <= i)
            {
                if (s[i] >= '0' && s[i] <= '9')
                {
                    ++len;
                }
                else
                {
                    break;
                }
                --i;
            }
            return len;
        }
    }
}
