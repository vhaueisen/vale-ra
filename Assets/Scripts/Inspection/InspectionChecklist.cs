using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace ARInspection
{
    [XmlRoot(ElementName = "item")]
    public class Item
    {
        [XmlElement(ElementName = "descricao")]
        public string Descricao { get; set; }
        [XmlElement(ElementName = "grupo")]
        public string Grupo { get; set; }
    }

    [XmlRoot(ElementName = "checklist")]
    public class InspectionChecklist
    {
        [XmlElement(ElementName = "tipo")]
        public string Tipo { get; set; }
        [XmlElement(ElementName = "modelo")]
        public string Modelo { get; set; }
        [XmlElement(ElementName = "tag")]
        public string Tag { get; set; }
        [XmlElement(ElementName = "item")]
        public List<Item> Item { get; set; }
    }
}
