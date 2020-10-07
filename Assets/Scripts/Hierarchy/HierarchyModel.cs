using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class HierarchyModel : ApplicationElement
{
    public HierarchyView view;
    public HierarchyNodeController currentViewing;
    public List<HierarchyNodeController> hierarchyNodeRoot = new List<HierarchyNodeController>();
    public List<HierarchyNodeController> hierarchyNodeList = new List<HierarchyNodeController>();
    public List<List<HierarchyNodeController>> hierarchyNodeSortedList = new List<List<HierarchyNodeController>>();
    public TextAsset socariaXml;
    public bool IsLoaded = false;
    public GameObject[] targets;
    public RectTransform mainPage;
    public RectTransform targetPage;
    public Vector2 mainPageInitialPosition;
    public Vector2 targetPageInitialPosition;
    public float animDuration = 0.25f;
    public float width;
    public float padding = 0;
    public GameObject nodePrefab;
    public RectTransform nodeContainer;
    public Text headerTitle;
    public List<GameObject> loadedNodes = new List<GameObject>();
    public ScrollRect scrollRect;
    public string ModelXml
    {
        get => m_modelXml;
        set
        {
            string oldModel = m_modelXml;
            m_modelXml = value;
            if (!string.Equals(oldModel, m_modelXml, StringComparison.OrdinalIgnoreCase))
                MainApp.hierarchyController.Initialize();
        }
    }
    private string m_modelXml = "";
    public bool LoadXML()
    {
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ModelXML), new XmlRootAttribute("model"));
            TextReader textReader = new StringReader(ModelXml);
            XmlTextReader xmlReader = new XmlTextReader(textReader);
            ModelXML xmlModel = (ModelXML)serializer.Deserialize(xmlReader);
            modelObject rootModel = xmlModel.Root;
            HierarchyNodeController root = new HierarchyNodeController();
            foreach (modelObject node in xmlModel.Root.ChildList)
                MainApp.hierarchyController.LoadHierarchyRecursively(node, root, 0);
            HierarchyNodeController.hierarchyModel = this;
            return true;
        }
        catch (System.Exception e)
        {
#if UNITY_EDITOR
            Debug.Log(e.ToString());
#endif
            return false;
        }
    }
}