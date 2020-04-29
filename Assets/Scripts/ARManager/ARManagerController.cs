using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARManagerController : ApplicationElement
{
    public ARPlaneManager planeManager;
    public GameObject occlusionPlane;
    public GameObject transparentPlane;

    private void Start()
    {
        if (!MainApp.coreDataModel.Settings.Core.IsOcclusion && MainApp.coreDataModel.Settings.IsLoaded)
            planeManager.planePrefab = transparentPlane;
        else
            planeManager.planePrefab = occlusionPlane;
    }
}