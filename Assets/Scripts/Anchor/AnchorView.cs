using UnityEngine.XR.ARFoundation;

public class AnchorView : ApplicationElement
{
    public ARPlaneManager arPlaneManager;
    void Start()
    {
        MainApp.toolBoxView.toolBoxEvent += OnToolBoxEvent;
    }

    void OnDestroy()
    {
        MainApp.toolBoxView.toolBoxEvent -= OnToolBoxEvent;
    }

    public void OnToolBoxEvent(object sender, ToolBoxEventArgs eventArgs)
    {
        if (eventArgs.ToolKey == ToolBoxEventArgs.anchorKey)
            TogglePlaneDetection(true);
        else
            TogglePlaneDetection(false);
    }

    private void TogglePlaneDetection(bool b)
    {
        if (arPlaneManager.enabled != b)
        {
            arPlaneManager.enabled = b;
            foreach (var plane in arPlaneManager.trackables)
                plane.gameObject.SetActive(b);
        }
    }
}