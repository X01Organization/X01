namespace Json2Class
{
    public class ClassInfo
    {
        public List<string> Usings { get; set; } = new List<string>();
        public string NameSpace { get; set; }
        public string Name { get; set; }
        public List<PropertyInfo1> Properties { get; set; } = new List<PropertyInfo1>();
    }
}