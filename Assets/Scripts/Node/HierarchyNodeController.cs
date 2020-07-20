using UnityEngine;
using System.Collections.Generic;

public class HierarchyNodeController
{
    public static HierarchyModel hierarchyModel;
    private HierarchyNodeView view;
    public RectTransform viewTransform;
    public int level;
    public string nodeName;
    public static bool selected = false;
    public static bool isolated = false;
    public bool isAssetHidden = false;
    public HierarchyNodeController parent;
    public static HierarchyNodeController isolatedNode = null;
    public static HierarchyNodeController highlightedNode = null;
    public static HierarchyNodeController[] allNodes;
    public static List<HierarchyNodeController> highlightedNodes = new List<HierarchyNodeController>();
    public List<HierarchyNodeController> childNodes = new List<HierarchyNodeController>();
    public List<GameObject> childAssets = new List<GameObject>();
    public static List<GameObject> highlightedObjects = new List<GameObject>();
    public static List<Material[]> highlightedObjectsMaterials = new List<Material[]>();
    public Material highlighMaterial;
    public Color highlightedColor;

    public HierarchyNodeController(modelObject model, HierarchyNodeView nodeView, HierarchyNodeController nodeParent, int nodeLevel)
    {
        level = nodeLevel;
        nodeName = model.Name;
        view = nodeView;
        parent = nodeParent;
        viewTransform = view.GetComponent<RectTransform>();
        view.Set(this);
        if (model.AssetList != null)
            foreach (string searchKey in model.AssetList)
            {
                GameObject searchResult = GameObject.Find(searchKey);
                if (searchResult != null)
                {
                    // HierarchyNodeView nodeView = searchResult.AddComponent(typeof(HierarchyMemberComponent)) as HierarchyMemberComponent;
                    // component.node = this;
                    childAssets.Add(searchResult);
                }
            }
    }

    public HierarchyNodeController()
    {
        nodeName = "Raiz";
    }

    private void DoToggleHide()
    {
        foreach (GameObject asset in childAssets)
        {
            MeshRenderer m = asset.GetComponent<MeshRenderer>();
            Collider c = asset.GetComponent<Collider>();
            if (m)
                m.enabled = !isAssetHidden;
            if (c)
                c.enabled = !isAssetHidden;
        }
        foreach (HierarchyNodeController node in childNodes)
            node.ToggleHide(isAssetHidden);
    }

    public void ToggleHide()
    {
        isAssetHidden = !isAssetHidden;
        DoToggleHide();
    }

    public void ToggleHide(bool b)
    {
        isAssetHidden = b;
        DoToggleHide();
    }

    private void hideAll()
    {
        foreach (HierarchyNodeController hnc in allNodes)
        {
            if (!hnc.isAssetHidden)
            {
                hnc.ToggleHide(false);
            }
        }
    }

    private void showAll()
    {
        foreach (HierarchyNodeController hnc in allNodes)
        {
            if (hnc.isAssetHidden)
            {
                hnc.ToggleHide(true);
            }
        }
    }

    // public void togglehighlightNode()
    // {
    //     foreach (HierarchyNode node in highlightedNodes)
    //     {
    //         node.tittle.color = Color.white;
    //         node.backgroundImage.color = new Color(0, 0, 0, 0.2f + 0.1f * node.kinship);
    //     }
    //     if (globalHighlightedState)
    //     {
    //         organizeViewport(this);
    //         highlightedNodes.Clear();
    //         highlightedNodes.Add(this);
    //         globalHighlightedNode = this;
    //         if (allChildNodes.Count > 0)
    //         {
    //             foreach (TreeNodeController tnc in allChildNodes)
    //             {
    //                 highlightedNodes.Add(tnc);
    //             }
    //         }
    //         foreach (TreeNodeController node in allChildNodes)
    //         {
    //             node.backgroundImage.color = new Color(0.25f, 0.6f, 0.3f, 0.5f);
    //         }
    //         backgroundImage.color = new Color(0.25f, 0.6f, 0.3f, 0.5f);
    //     }
    // }

    // private void highlight()
    // {
    //     globalSavedMaterials.Clear();
    //     highlightedObjects.Clear();
    //     foreach (GameObject asset in allAssetList)
    //     {
    //         Material[] materialsList = asset.GetComponent<Renderer>().materials;
    //         Material[] highlightList = asset.GetComponent<Renderer>().materials;
    //         globalSavedMaterials.Add(materialsList);
    //         highlightedObjects.Add(asset);
    //         for (int i = 0; i < highlightList.Length; i++)
    //         {
    //             highlightList[i] = highlighMaterial;
    //         }
    //         asset.GetComponent<Renderer>().materials = highlightList;
    //         asset.layer = 1;
    //     }
    // }

    // public static void unHighlight()
    // {
    //     for (int i = 0; i < globalSavedMaterials.Count; i++)
    //     {
    //         highlightedObjects[i].GetComponent<Renderer>().materials = globalSavedMaterials[i];
    //         highlightedObjects[i].layer = 0;
    //     }
    //     globalSavedMaterials.Clear();
    //     highlightedObjects.Clear();
    // }

    // public void toggleIsolate()
    // {
    //     if (isolated && this != isolatedNode)
    //         isolatedNode.toggleIsolate();

    //     isolated = !isolated;

    //     if (isolated)
    //     {
    //         hideAll();
    //         foreach (TreeNodeController tnc in allChildNodes)
    //         {
    //             tnc.isAssetIsolated = true;
    //             tnc.isAssetHidden = false;
    //         }
    //         toggleHide();
    //         isolateGlobalState = true;
    //         globalIsolatedNode = this;
    //     }
    //     else
    //     {
    //         foreach (TreeNodeController tnc in allChildNodes)
    //         {
    //             tnc.isAssetIsolated = false;
    //         }
    //         showAll();
    //         UpdateStatus();
    //         isolateGlobalState = false;
    //     }
    //     updateAll();

    //     TreenodeStatesController.refreshStateList = true;
    // }

    public void Pressed()
    {
        if (childNodes == null || childNodes.Count == 0)
            return;

        hierarchyModel.currentViewing = this;
        hierarchyModel.view.SlideToNext(hierarchyModel.currentViewing.childNodes);
        hierarchyModel.view.OnNodeChange();
    }
}