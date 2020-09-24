using UnityEngine;

public class HierarchyController : MonoBehaviour
{
    public HierarchyModel model;

    public void LoadHierarchyRecursively(modelObject node, HierarchyNodeController parent, int level)
    {
        GameObject nodeInstance = Instantiate(model.nodePrefab, Vector3.zero, Quaternion.identity, level == 0 ? model.mainPage : model.nodeContainer);
        model.loadedNodes.Add(nodeInstance);
        HierarchyNodeView nodeView = nodeInstance.GetComponent<HierarchyNodeView>();
        HierarchyNodeController recursiveNode = new HierarchyNodeController(node, nodeView, parent, level);
        if (model.currentViewing == null)
            model.currentViewing = recursiveNode;
        if (level == 0)
            model.hierarchyNodeRoot.Add(recursiveNode);

        if (parent != null)
            parent.childNodes.Add(recursiveNode);

        if (node.ChildList != null && node.ChildList.Length > 0)
            foreach (modelObject recursiveModel in node.ChildList)
            {
                LoadHierarchyRecursively(recursiveModel, recursiveNode, level + 1);
            }
    }

    public void Initialize()
    {
        model.IsLoaded = model.LoadXML();
        model.mainPageInitialPosition = model.mainPage.anchoredPosition;
        model.targetPageInitialPosition = model.targetPage.anchoredPosition;
        model.width = model.mainPage.rect.width * 1.075f;
    }
}