using System.Xml;

namespace XmlToModel
{
    public class XmlParser
    {
        public void Parse(XmlNode node)
        {
            var rr = new Dictionary<string, string>() {
               {"Angebotsnummer","quotenumber"},
{"Saison","season"},
{"Name","Surname"},
{"GueltigVon","Validfrom"},
{"GueltigBis","DateofExpiry"},
{"Typ","Type"},
{"GiataID","GiataID"},
{"DLC","DLC"},
{"RegionDLC","RegionDLC"},
{"LandDLC","CountryDLC"},
{"Kategorie","category"},
{"Katalog","Catalog"},
{"Adresse","address"},
{"Details","details"},
{"Bilder","pictures"},

{"Ort","location"},
{"Lage","Position"},
{"MobilvorOrt","Mobileonsite"},
{"Ausstattung","Furnishing"},
{"GenießenSchlemmen","EnjoyFeasting"},
{"BeautyWellnessFreizeit","BeautyWellnessLeisure"},
{"Freizeit","leisure"},

{"Teaser","teaser"},
{"Einleitung","introduction"},
{"Beschreibung","description"},
{"Wohnen","Reside"},
{"Leistung","service"},
{"Zusatzinformationen","additionalInformation"},
{"Kinder","children"},
{"Sparen","saving"},
{"Programm","program"},

{"path","Path"},
{"src","Source"},
{"pos","position"},
{"alt","alt"},
{"title","title"},
{"copyright","copyright"},
{"type","type"},
           };

            var namess1 =
 node.Attributes.Cast<XmlAttribute>()
                .Select(x => x.Name).ToArray();

            var names11 = string.Join(Environment.NewLine, node.Attributes.Cast<XmlAttribute>()
                .Select(x => x.Name)
                .Select(x => $"/// <summary>\r\n/// {x}\r\n///</summary>\r\n[XmlAttribute(\"{x}\")]\r\npublic string {rr[x].Substring(0, 1).ToUpperInvariant() + rr[x].Substring(1)}"
                + "{get;set;}")
                );


            var namess =
 node.ChildNodes.Cast<XmlNode>()
                .Select(x => x.Name).ToArray();



            //[XmlAttribute("catalogID")]
            var names = string.Join(Environment.NewLine, node.ChildNodes.Cast<XmlNode>()
                .Select(x => x.Name)
                .Select(x => $"/// <summary>\r\n/// {x}\r\n///</summary>\r\n[XmlElement(\"{x}\")]\r\npublic string {rr[x].Substring(0, 1).ToUpperInvariant() + rr[x].Substring(1)}"
                + "{get;set;}")
                );
            int a = 0;
        }
    }
}
