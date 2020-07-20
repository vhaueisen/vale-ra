using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HierarchyModel : MonoBehaviour
{
    public HierarchyController controller;
    public HierarchyView view;
    public HierarchyNodeController currentViewing;
    public List<HierarchyNodeController> hierarchyNodeRoot = new List<HierarchyNodeController>();
    public List<HierarchyNodeController> hierarchyNodeList = new List<HierarchyNodeController>();
    public List<List<HierarchyNodeController>> hierarchyNodeSortedList = new List<List<HierarchyNodeController>>();
    public TextAsset modelXML;
    public bool IsLoaded = false;
    public GameObject[] targets;
    public RectTransform mainPage;
    public RectTransform targetPage;
    public Vector2 mainPageInitialPosition;
    public Vector2 targetPageInitialPosition;
    public float animDuration = 0.25f;
    public float width;
    public float padding = 60;
    public GameObject nodePrefab;
    public RectTransform nodeContainer;
    public Text headerTitle;

    void Start()
    {
        IsLoaded = LoadXML();
        mainPageInitialPosition = mainPage.anchoredPosition;
        targetPageInitialPosition = targetPage.anchoredPosition;
        width = mainPage.rect.width;
        if (IsLoaded)
            controller.Initialize();
    }

    private bool LoadXML()
    {
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ModelXML), new XmlRootAttribute("model"));
            TextReader textReader = new StringReader(modelXML.text);
            XmlTextReader xmlReader = new XmlTextReader(textReader);
            ModelXML xmlModel = (ModelXML)serializer.Deserialize(xmlReader);
            modelObject rootModel = xmlModel.Root;
            HierarchyNodeController root = new HierarchyNodeController();
            foreach (modelObject node in xmlModel.Root.ChildList)
                controller.LoadHierarchyRecursively(node, root, 0);
            HierarchyNodeController.hierarchyModel = this;
            return true;
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
            return false;
        }
    }
}