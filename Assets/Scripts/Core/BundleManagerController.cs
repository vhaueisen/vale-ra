using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using static InventoryModel;

public class BundleManagerController : ApplicationElement
{
    public static bool LoadScript(string filepath, out ARObjectScript script)
    {
        AssetBundle bundle = AssetBundle.LoadFromFile(filepath);
        script = new ARObjectScript();
        if (bundle == null)
            return false;

        string folderPath = Path.GetDirectoryName(filepath);
        StringBuilder s = new StringBuilder();
        string manifestPath = Path.Combine(folderPath, bundle.name + ".manifest");
        if (File.Exists(manifestPath))
        {
            string[] text = File.ReadAllLines(manifestPath);
            string addr = "";
            foreach (string t in text)
            {
                if (t.Contains("ARObjectModel.prefab"))
                {
                    addr = t.Substring(2);
                }
            }
            if (addr.Length > 0)
            {
                GameObject asset = bundle.LoadAsset(addr) as GameObject;
                if (asset != null)
                {
                    ARObejctModel _model = asset.GetComponent<ARObejctModel>();
                    if (_model != null)
                    {
                        script.Clone(_model);
                        script.bundlePath = filepath;
                        script.addr = addr;
                        script.id = bundle.name;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void LoadObject(ARObjectScript model)
    {
        StartCoroutine(LoadObjectAsync(model));
    }

    private IEnumerator LoadObjectAsync(ARObjectScript model)
    {
        MainApp.inventoryController.ShowLoading();
        string BundlePath;
        string BundleAddress;
        string BundleId;

        if (model == null && MainApp.coreDataModel.Inventory.IsLoaded)
        {
            InventorySettings.InventoryData Core = MainApp.coreDataModel.Inventory.Core;
            BundlePath = Core.BundlePath;
            BundleAddress = Core.Addr;
            BundleId = Core.Id;
        }
        else if (model != null)
        {
            BundlePath = model.bundlePath;
            BundleAddress = model.addr;
            BundleId = model.id;
        }
        else
        {
            MainApp.inventoryController.HideLoading();
            yield break;
        }

        AssetBundle.UnloadAllAssetBundles(true);
        yield return null;

        string parentDir = Directory.GetParent(BundlePath).ToString();
        string dependencyPath = Path.Combine(parentDir, new DirectoryInfo(parentDir).Name);
        if (File.Exists(dependencyPath))
        {
            AssetBundle assetBundle = AssetBundle.LoadFromFile(dependencyPath);
            AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            yield return null;

            string[] dependencies = manifest.GetAllDependencies(BundleId);
            foreach (string dependency in dependencies)
            {
                AssetBundleCreateRequest dependencyRequest = AssetBundle.LoadFromFileAsync(
                    Path.Combine(Directory.GetParent(BundlePath).ToString(), dependency)
                );
                while (!dependencyRequest.isDone)
                    yield return null;
            }
        }
        if (File.Exists(BundlePath))
        {
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

            if (SceneLoaderModel.CurrentScene.sceneIndex == SceneLoaderModel.ARScene.sceneIndex)
            {
                MainApp.notificationComponent.Notify(
                    loadModel.ARImage ? "Aproxime câmera do seu dispositivo da imagem" : "Pressione no local de ancoragem do objeto"
                );
            }
            else
                FindObjectOfType<ProjectionController>().HomeProjection();
            MainApp.toolBoxView.ChangeObject(loadModel);
        }
        MainApp.inventoryController.HideLoading();
        yield break;
    }

    public bool ValidateBundle(AssetBundle bundle, string folderPath, string bundlePath, out ARObjectScript model)
    {
        model = new ARObjectScript();
        if (bundle == null)
            return false;

        StringBuilder s = new StringBuilder();
        string manifestPath = Path.Combine(folderPath, bundle.name + ".manifest");
        if (File.Exists(manifestPath))
        {
            string[] text = File.ReadAllLines(manifestPath);
            string addr = "";
            foreach (string t in text)
            {
                if (t.Contains("ARObjectModel.prefab"))
                {
                    addr = t.Substring(2);
                }
            }
            if (addr.Length > 0)
            {
                GameObject asset = bundle.LoadAsset(addr) as GameObject;
                if (asset != null)
                {
                    ARObejctModel _model = asset.GetComponent<ARObejctModel>();
                    if (_model != null)
                    {
                        model.Clone(_model);
                        model.bundlePath = bundlePath;
                        model.addr = addr;
                        model.id = bundle.name;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void LoadInventoryItemsData()
    {
        string path = Path.Combine(InventoryModel.bundlePath, InventoryModel.bundleFolder);
        if (Directory.Exists(path))
        {
            List<string> fileEntries = Directory.GetDirectories(path).ToList();
            fileEntries.Add(path);
            foreach (string fileLocation in fileEntries)
            {
                string[] fileList = Directory.GetFiles(fileLocation);
                foreach (string file in fileList)
                {
                    if (!file.Contains(".manifest"))
                    {
                        DirectoryInfo fileInfo = new DirectoryInfo(file);
                        if (File.Exists(file) && fileInfo.Name != Directory.GetParent(file).Name)
                        {
                            try
                            {
                                AssetBundle bundle = AssetBundle.LoadFromFile(file);
                                ARObjectScript loadedScript;
                                if (ValidateBundle(bundle, fileLocation, file, out loadedScript))
                                    MainApp.inventoryController.AddressBundle(loadedScript);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
        }
    }

    public bool LoadInventoryItemData(string file, bool createConteiner = false)
    {
        try
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(file);
            if (bundle != null)
            {
                ARObjectScript model;
                if (MainApp.bundleManager.ValidateBundle(bundle, Path.GetDirectoryName(file), file, out model))
                {
                    GameObject itemInstance = Instantiate(MainApp.inventoryModel.inventoryItem,
                            Vector3.zero,
                            Quaternion.identity,
                            MainApp.inventoryModel.containerList[0].transform);
                    InventoryItemController itemController = itemInstance.GetComponent<InventoryItemController>();
                    if (itemController)
                    {
                        itemController.Initialize(model);
                        MainApp.inventoryModel.bucketList.Add(new ItemBucket(itemInstance, model));
                        MainApp.inventoryController.AddressBundle(model);
                        int containerSize = MainApp.inventoryModel.containerList.Count;
                        int newContainerSize = MainApp.inventoryModel.ARObjectBuckets.Count;
                        newContainerSize = MainApp.inventoryModel.ARObjectAreas.Count < containerSize ? containerSize : MainApp.inventoryModel.ARObjectAreas.Count;
                        if (containerSize != newContainerSize && !createConteiner)
                        {
                            MainApp.inventoryController.NewConteiner();
                            itemInstance.transform.SetParent
                            (
                                MainApp.inventoryModel.containerList[MainApp.inventoryModel.containerList.Count - 1].transform
                            );
                            return true;
                        }
                    }
                    else
                        Destroy(itemInstance);
                }
            }
        }
        catch
        {

        }
        return false;
    }
}