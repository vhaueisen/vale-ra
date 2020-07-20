using UnityEngine;
using System.Collections.Generic;

public class HierarchyController : MonoBehaviour
{
    public HierarchyModel model;

    public void LoadHierarchyRecursively(modelObject node, HierarchyNodeController parent, int level)
    {
        GameObject nodeInstance = Instantiate(model.nodePrefab, Vector3.zero, Quaternion.identity, level == 0 ? model.mainPage : model.nodeContainer);
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

    }
}