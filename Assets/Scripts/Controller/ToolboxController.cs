using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ToolboxController : ApplicationElement
{
    private ToolboxModel model;
    private void Start()
    {
        model = MainApp.toolboxModel;
        model.containerTransform = model.container.GetComponent<RectTransform>();
        RefreshChildren();
    }

    private void RefreshChildren()
    {
        model.containerBtnList = model.container.GetComponentsInChildren<Image>().Skip(1).Select(o => o.gameObject).ToArray();
        model.openSize = model.openSize * model.containerBtnList.Length;
    }
}
