namespace ComputopParameterParser.Data
{
    public struct JsonSchemaInfo
    {
        public Type Type
        {
            get;
            set;
        }
        public string Pattern
        {
            get;
            set;
        }

        public bool IsRequired
        {
            get;
            set;
        }
    }
}
