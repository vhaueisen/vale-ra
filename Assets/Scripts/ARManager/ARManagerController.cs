using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class ARManagerController : ApplicationElement
{
    public ARPlaneManager planeManager;
    public AROcclusionManager occlusionManager;
    public GameObject occlusionPlane;
    public GameObject transparentPlane;

    private void Start()
    {
        occlusionManager.enabled = false;
        if (MainApp.coreDataModel.Settings.IsLoaded)
            if (MainApp.coreDataModel.Settings.Core.IsOcclusion)
                occlusionManager.enabled = true;
    }
}