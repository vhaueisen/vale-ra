using System;
using System.Collections;
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
        StartCoroutine(View());
    }

    private IEnumerator View()
    {
        MainApp.inventoryController.ShowLoading();
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
        yield return null;
        AssetBundle.UnloadAllAssetBundles(true);
        yield return null;
        AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(BundlePath);
        while (!bundleRequest.isDone)
            yield return null;

        AssetBundleRequest prefabRequest = bundleRequest.assetBundle.LoadAssetAsync(BundleAddress);
        while (!prefabRequest.isDone)
            yield return null;

        ARObejctModel loadModel = (prefabRequest.asset as GameObject).GetComponent<ARObejctModel>();
        if (loadModel.ARImage && FindObjectOfType<ProjectionModel>().ARMode)
        {
            try
            {

                FindObjectOfType<ARTrackedImageManager>().referenceLibrary = loadModel.referenceImageLibrary;
                FindObjectOfType<ARTrackedImageManager>().trackedImagePrefab = loadModel.ARPrefab;
                FindObjectOfType<ARTrackedImageManager>().enabled = true;
            }
            catch
            {

            }
        }
        else
            MainApp.inventoryModel.ChangeProjection(loadModel);

        yield return null;
        WindowComponent[] windows = FindObjectsOfType<WindowComponent>();
        foreach (WindowComponent window in windows)
            window.Exit();
        UpdateVars();
        if (SceneLoaderModel.CurrentScene.sceneIndex == SceneLoaderModel.ARScene.sceneIndex)
        {
            MainApp.notificationComponent.Notify(loadModel.ARImage ? "Aproxime câmera do seu dispositivo da imagem" : "Pressione e segure no local de ancoragem do objeto");
        }
        else
            FindObjectOfType<ProjectionController>().HomeProjection();

        MainApp.inventoryController.HideLoading();
        MainApp.toolBoxView.ChangeObject(loadModel);
        yield break;
    }
}