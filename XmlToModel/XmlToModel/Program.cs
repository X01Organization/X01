// See https://aka.ms/new-console-template for more information
using System.Xml;
using XmlToModel;

var xmlFileFullName = @"C:\Temp\3.xml";
var xmlContent = File.ReadAllText(xmlFileFullName)
    .Replace("env:","").Replace("ns1:","")
    ;
var xDoc = new XmlDocument();
xDoc.LoadXml(xmlContent);
XmlNamespaceManager ns = new XmlNamespaceManager(  xDoc.NameTable);
ns.AddNamespace("env:","http://www.w3.org/2003/05/soap-envelope");
ns.AddNamespace("ns1:","https://service.ameropa.de/nbc/nbc.wsdl");
//new XmlParser().Parse(xDoc.SelectSingleNode("env:Envelope", ns));
new XmlParser().Parse(xDoc.SelectSingleNode("image" ));
