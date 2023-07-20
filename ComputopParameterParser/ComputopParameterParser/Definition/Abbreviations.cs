using ComputopParameterParser.Data;

namespace ComputopParameterParser.Definition
{
    /// <summary>
    /// @ref https://developer.computop.com/pages/viewpage.action?pageId=26247171
    /// </summary>
    public sealed class Abbreviations
    {
        private static readonly Dictionary<string, JsonSchemaInfo> _dict = new Dictionary<string, JsonSchemaInfo>()
        {
            {"CND", new JsonSchemaInfo() {IsRequired = false}},
            {"M", new JsonSchemaInfo() {IsRequired = true}},
            {"O", new JsonSchemaInfo() {IsRequired = false}},
            {"C", new JsonSchemaInfo() {IsRequired = false}},
            {"OM", new JsonSchemaInfo() {IsRequired = false}},
            {"OC", new JsonSchemaInfo() {IsRequired = false}},
        };

        public static JsonSchemaInfo GetJsonSchemaInfo(DefinitioinInfo di)
        {
            return _dict[di.Condition];
        }
    }
}