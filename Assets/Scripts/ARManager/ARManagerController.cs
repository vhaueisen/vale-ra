using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARManagerController : ApplicationElement
{
    public ARPlaneManager planeManager;
    public AROcclusionManager occlusionManager;
    public GameObject occlusionPlane;
    public GameObject transparentPlane;
    public LightEstimationComponent LightEstimationComponent;
    private void Start()
    {
        occlusionManager.enabled = false;
        LightEstimationComponent.enabled = false;
        if (MainApp.coreDataModel.Settings.IsLoaded)
        {
            occlusionManager.enabled = MainApp.coreDataModel.Settings.Core.IsOcclusion;
            LightEstimationComponent.enabled = MainApp.coreDataModel.Settings.Core.EstimatingLight;
        }
    }
}