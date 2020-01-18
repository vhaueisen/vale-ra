using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

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

    public void ViewObject()
    {
        window.Exit();
        AssetBundle.UnloadAllAssetBundles(true);
        string parentDir = Directory.GetParent(tempModel.bundlePath).ToString();
        AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(parentDir, new DirectoryInfo(parentDir).Name));
        AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        string[] dependencies = manifest.GetAllDependencies(tempModel.id);
        foreach (string dependency in dependencies)
        {
            AssetBundle.LoadFromFile(Path.Combine(Directory.GetParent(tempModel.bundlePath).ToString(), dependency));
        }

        AssetBundle bundle = AssetBundle.LoadFromFile(tempModel.bundlePath);
        GameObject asset = bundle.LoadAsset(tempModel.addr) as GameObject;
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