using System;
using System.IO;
using System.Xml.Serialization;

public static class XMLReader
{
    public static T FromXml<T>(string xml)
    {
        T xmlObj = default(T);
        try
        {
            using (TextReader reader = new StringReader(xml))
            {
                try
                {
                    xmlObj =
                        (T)new XmlSerializer(typeof(T)).Deserialize(reader);
                }
                catch
                {
                }
            }
        }
        catch
        {
        }
        return xmlObj;
    }
}