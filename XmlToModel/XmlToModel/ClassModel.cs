using System.Collections.ObjectModel;

namespace XmlToModel
{
    public class ClassModel
    {
        public string Namespace
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        public ICollection<PropertyModel> Properties
        {
            get;
        } = new Collection<PropertyModel>();
    }
}
