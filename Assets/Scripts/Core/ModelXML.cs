[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public class ModelXML
{
    private string nameField;
    private modelAnimation[] animationField;
    private modelObject objectField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("name")]
    public string Name
    {
        get
        {
            return this.nameField;
        }
        set
        {
            this.nameField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("animation")]
    public modelAnimation[] AnimationList
    {
        get
        {
            return this.animationField;
        }
        set
        {
            this.animationField = value;
        }
    }
    [System.Xml.Serialization.XmlElementAttribute("object")]
    /// <remarks/>
    public modelObject Root
    {
        get
        {
            return this.objectField;
        }
        set
        {
            this.objectField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class modelAnimation
{
    private string idField;

    private string nameField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("id")]
    public string Id
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("name")]
    public string Name
    {
        get
        {
            return this.nameField;
        }
        set
        {
            this.nameField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class modelObject
{
    private string idField;
    private string nameField;
    private modelObject[] objectField;
    private string[] assetField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("id")]
    /// <remarks/>
    public string Id
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("name")]
    public string Name
    {
        get
        {
            return this.nameField;
        }
        set
        {
            this.nameField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("object")]
    public modelObject[] ChildList
    {
        get
        {
            return this.objectField;
        }
        set
        {
            this.objectField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("asset")]
    public string[] AssetList
    {
        get
        {
            return this.assetField;
        }
        set
        {
            this.assetField = value;
        }
    }
}