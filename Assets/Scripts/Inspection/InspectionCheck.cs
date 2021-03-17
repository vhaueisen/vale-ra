using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot(ElementName = "checklist")]
public class InspectionCheck
{
    [XmlElement(ElementName = "tipo")]
    public string Tipo { get; set; }
    [XmlElement(ElementName = "modelo")]
    public string Modelo { get; set; }
    [XmlElement(ElementName = "tag")]
    public string Tag { get; set; }
    [XmlElement(ElementName = "item")]
    public List<string> Item { get; set; }
}