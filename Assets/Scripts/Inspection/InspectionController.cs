using UnityEngine;

public class InspectionController : ApplicationElement
{
    private InspectionModel model;
    public TextAsset textXML;
    private TextAsset previousTextXML = null;
    public RectTransform parent;
    public GameObject InspectionItem;
    private InspectionCheck check;
    public void OnInspection()
    {
        DestroyItems();
        check = ReadXML();
        InstantiateItens();
    }

    private void DestroyItems()
    {
        if (previousTextXML != textXML)
            for (int i = 0; i < 0; i++)
            {
                Destroy(parent.GetChild(i));
            }
        previousTextXML = textXML;
    }

    private InspectionCheck ReadXML()
    {
        return XMLReader.FromXml<InspectionCheck>(textXML.text);
    }

    private void InstantiateItens()
    {
        foreach (string name in check.Item)
        {
            GameObject item = Instantiate(InspectionItem, parent);
            item.GetComponent<InspectionItem>().Initialize(name);
        }
    }
}