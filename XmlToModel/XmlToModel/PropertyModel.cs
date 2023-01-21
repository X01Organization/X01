using System.Collections.ObjectModel;

namespace XmlToModel
{
    public class PropertyModel
    {
        public Type Type
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        public ICollection<string> Attributes
        {
            get;
        } = new Collection<string>();
    }
}
