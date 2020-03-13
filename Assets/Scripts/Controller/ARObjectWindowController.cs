using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using static DataModel;
public class ARObjectWindowController : ApplicationElement
{
    public Text tittle;
    public Text description;
    public Image header;
    public Text area;
    public WindowComponent window;
    private ARObjectScript tempModel;
    public static ARObjectScript selectedModel = null;

    public void EnterARWindow(ARObjectScript model)
    {
        if (model == null || !window)
            return;

        tempModel = model;
        window.Enter();
        area.text = model.Area;
        tittle.text = model.Name;
        description.text = model.Description;
        header.sprite = model.Image;
    }

    private void UpdateVars()
    {
        InventorySettings.InventoryData core = new InventorySettings.InventoryData();
        core.Addr = tempModel.addr;
        core.BundlePath = tempModel.bundlePath;
        core.Id = tempModel.id;
        DataEventArgs eventArgs = new DataEventArgs(DataEventArgs.UpdateEvent, core);
        MainApp.coreDataModel.Inventory.OnSettingsEvent(this, eventArgs);
    }
    public void ViewObject()
    {
        string BundlePath;
        string BundleAddress;
        string BundleId;

        if (tempModel == null && MainApp.coreDataModel.Inventory.IsLoaded)
        {
            InventorySettings.InventoryData Core = MainApp.coreDataModel.Inventory.Core;
            BundlePath = Core.BundlePath;
            BundleAddress = Core.Addr;
            BundleId = Core.Id;
        }
        else
        {
            BundlePath = tempModel.bundlePath;
            BundleAddress = tempModel.addr;
            BundleId = tempModel.id;
        }

        AssetBundle.UnloadAllAssetBundles(true);
        string parentDir = Directory.GetParent(BundlePath).ToString();
        AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(parentDir, new DirectoryInfo(parentDir).Name));
        AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        string[] dependencies = manifest.GetAllDependencies(BundleId);
        foreach (string dependency in dependencies)
        {
            AssetBundle.LoadFromFile(Path.Combine(Directory.GetParent(BundlePath).ToString(), dependency));
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
            MainApp.inventoryModel.ChangeProjection(loadModel.ARPrefab);

        WindowComponent[] windows = FindObjectsOfType<WindowComponent>();
        foreach (WindowComponent window in windows)
            window.Exit();
        FindObjectOfType<ToastNotificationComponent>().Notify("Pressione e segure no local de ancoragem do objeto");
        UpdateVars();
    }
}