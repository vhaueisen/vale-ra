#pragma warning disable 0168
#pragma warning disable 0219
#pragma warning disable 0414

using System.IO;
using UnityEngine;
using System;
using UnityEngine.XR.ARFoundation;

[System.Serializable]
public class UserSettings : DataModel
{
    [System.Serializable]
    public struct UserData
    {
        public float ScaleSpeed;
        public float TranslateSpeed;
        public float RotateSpeed;
        public float ElevateSpeed;
        public bool IsOcclusion;
    }
    public UserData Core = new UserData();
    public override void Load()
    {
        if (p_Load() && data != null)
        {
            Core = (UserData)data;
            IsLoaded = true;
        }
    }
}

[System.Serializable]
public class InventorySettings : DataModel
{
    [System.Serializable]
    public struct InventoryData
    {
        public string BundlePath;
        public string Addr;
        public string Id;

    }
    public InventoryData Core = new InventoryData();
    public override void Load()
    {
        if (p_Load() && data != null)
        {
            Core = (InventoryData)data;
            IsLoaded = true;
        }
    }
}

public class CoreDataModel : ApplicationElement
{
    public UserSettings Settings;
    public InventorySettings Inventory;

    void Start()
    {
        Settings = new UserSettings();
        Settings.DataName = Application.persistentDataPath + "/" + "FFF";
        Settings.DataEvent += Settings.OnSettingsEvent;
        Settings.Load();

        Inventory = new InventorySettings();
        Inventory.DataName = Application.persistentDataPath + "/" + "CCC";
        Inventory.DataEvent += Inventory.OnSettingsEvent;
        Inventory.Load();

        if (Inventory.IsLoaded)
        {
            string BundlePath;
            string BundleAddress;
            string BundleId;
            BundlePath = Inventory.Core.BundlePath;
            BundleAddress = Inventory.Core.Addr;
            BundleId = Inventory.Core.Id;

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
        }
    }
}