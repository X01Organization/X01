namespace ComputopParameterParser.Data
{
    public sealed class DefinitioinInfo
    {
        public string Name
        {
            get;
        }
        public string Format
        {
            get;
        }
        public string Condition
        {
            get;
        }
        public string Description
        {
            get;
        }
        public DefinitioinInfo(string name, string format, string condition, string description)
        {
            Name = name;
            Format = format;
            Condition = condition;
            Description = description;
        }
    }
}
