using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ProjectionController : ProjectionManipulator
{
    private byte currentState = 0;
    private void Start()
    {
        MainApp.touchView.TouchStateMachine += OnTouchStateChange;
        MainApp.inventoryModel.InventoryStateMachine += OnProjectionStateChange;
        Initialize();
        StartCoroutine("loadAsync");
    }

    private IEnumerator loadAsync()
    {
        yield return new WaitForEndOfFrame();
        string BundlePath;
        string BundleAddress;
        string BundleId;
        BundlePath = MainApp.coreDataModel.Inventory.Core.BundlePath;
        BundleAddress = MainApp.coreDataModel.Inventory.Core.Addr;
        BundleId = MainApp.coreDataModel.Inventory.Core.Id;

        yield return new WaitForEndOfFrame();
        string parentDir = Directory.GetParent(BundlePath).ToString();
        try
        {
            AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(parentDir, new DirectoryInfo(parentDir).Name));
            AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            string[] dependencies = manifest.GetAllDependencies(BundleId);
            foreach (string dependency in dependencies)
                AssetBundle.LoadFromFile(Path.Combine(Directory.GetParent(BundlePath).ToString(), dependency));
        }
        catch
        {
            HomeProjection();
            yield break;
        }

        AssetBundle bundle = AssetBundle.LoadFromFile(BundlePath);
        GameObject asset = bundle.LoadAsset(BundleAddress) as GameObject;
        ARObejctModel loadModel = asset.GetComponent<ARObejctModel>();
        if (loadModel.ARImage && FindObjectOfType<ProjectionModel>().ARMode)
        {
            try
            {

                FindObjectOfType<ARTrackedImageManager>().referenceLibrary = loadModel.referenceImageLibrary;
                FindObjectOfType<ARTrackedImageManager>().trackedImagePrefab = loadModel.ARPrefab;
                FindObjectOfType<ARTrackedImageManager>().enabled = true;
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
        else
            MainApp.inventoryModel.ChangeProjection(loadModel);
        HomeProjection();
        yield break;
    }

    public void OnTouchStateChange(object sender, TouchEventArgs eventArgs)
    {
        currentState = eventArgs.currentState;
#if (UNITY_ANDROID || UNITY_IOS)
        if (currentState == TouchModel.LongPressing)
            Handheld.Vibrate();
#endif
    }

    public void OnProjectionStateChange(object sender, InventoryEventArgs eventArgs)
    {
        if (projectionModel.CurrentInstance != null)
            Destroy(projectionModel.CurrentInstance);
        MainApp.inventoryModel.ProjectionPrefab = eventArgs.Prefab;
    }

    private void Update()
    {
        if (currentState > 0)
        {
            if (currentState == TouchModel.Pinching)
                UpdateScale(MainApp.touchModel.PinchAmount, projectionModel.ModelContainer, projectionModel.ARMode);
            else if (currentState == TouchModel.Swiping)
                UpdateRotation(-MainApp.touchModel.SwipeAmount.x, projectionModel.RotateComponent, projectionModel.ARMode);
            else if (currentState == TouchModel.LongPressing)
            {
                FlexibleRaycast raycast;
                if (projectionModel.ARMode)
                {
                    raycast = Raycast(MainApp.touchModel.TouchPosition, projectionModel.RayManager, projectionModel.MainCamera);
                }
                else
                    raycast = Raycast(MainApp.touchModel.TouchPosition, projectionModel.MainCamera);
                if (!(projectionModel.CurrentInstance == null))
                {
                    UpdatePosition(raycast, projectionModel.ModelContainer, projectionModel.ARMode);
                }
                else
                {
                    projectionModel.CurrentInstance = InstantiateProjection(
                        raycast, MainApp.inventoryModel.ProjectionPrefab,
                        projectionModel.RotateComponent,
                        projectionModel.ModelContainer);
                }
            }
            else if (currentState == TouchModel.Elevating && projectionModel.ARMode)
            {
                Elevate(projectionModel.RotateComponent, MainApp.touchModel.SwipeAmount.y);
            }
        }
    }

    public void HomeProjection()
    {
        FlexibleRaycast raycast = new FlexibleRaycast(HomeApp.projectionModel.HomeOrigin.transform.position,
        HomeApp.projectionModel.HomeOrigin.transform.rotation);
        projectionModel.CurrentInstance = InstantiateProjection(raycast, MainApp.inventoryModel.ProjectionPrefab, projectionModel.RotateComponent, projectionModel.ModelContainer);
    }
}