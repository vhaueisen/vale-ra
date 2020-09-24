using UnityEngine;
using UnityEngine.UI;

public class HierarchyPopupView : MonoBehaviour
{
    public GameObject content;
    public Image overlay;
    public RectTransform overlayTransform;
    public Text nodeTitle;
    private Color overlayColor = new Color(0.0f, 0.0f, 0.0f, 0.5f);
    void Start()
    {
        LeanTween.alpha(overlayTransform, 0f, 0f);
        HierarchyNodeView.NodeEvent += OnNodeViewEvent;
    }

    private void OnNodeViewEvent(object sender, NodeViewEventArgs eventArgs)
    {
        content.SetActive(true);
        nodeTitle.text = eventArgs.Node.nodeName;
        LeanTween.alpha(overlayTransform, 1.0f, 0.3f);
    }

    public void Exit()
    {
        LeanTween.alpha(overlayTransform, 0.0f, 0.3f).setOnComplete(() => content.SetActive(false));
    }
}
