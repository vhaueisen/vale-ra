#pragma warning disable 0168
#pragma warning disable 0219
#pragma warning disable 0414

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using System;
using UnityEngine.UI;

public class InventoryEventArgs : EventArgs
{
    public InventoryEventArgs(GameObject prefab)
    {
        Prefab = prefab;
    }
    public GameObject Prefab;
}

public class InventoryModel : ApplicationElement
{
    private static string bundleFolder = @"Bundles";
    private static string bundlePath;
    public GameObject inventoryItem;
    public GameObject inventoryContainer;
    public GameObject[] containerList;
    public InventoryContainerComponent[] containerComponentList;
    public Transform VerticalAlignedContent;
    public Transform GridAlignedContent;
    public ScrollRect ScrollRect;
    public List<ARObjectScript> ARObjectModels = new List<ARObjectScript>();
    public List<string> ARObjectNames = new List<string>();
    public List<string> ARObjectAreas = new List<string>();
    public List<string> ARObjectBuckets = new List<string>();
    public ARObejctModel CurrentModel;
    public List<Sprite> ARObjectThumbnails = new List<Sprite>();
    public event EventHandler<InventoryEventArgs> InventoryStateMachine;
    public GameObject ProjectionPrefab;

    protected virtual void OnStateChange(GameObject prefab)
    {
        if (InventoryStateMachine != null)
            InventoryStateMachine(this, new InventoryEventArgs(prefab));
    }

    public void ChangeProjection(ARObejctModel currentModel)
    {
        CurrentModel = currentModel;
        OnStateChange(currentModel.ARPrefab);
    }
    void Awake()
    {
        bundlePath = Application.persistentDataPath;
        LocateBundle();
    }

    private void LocateBundle()
    {
        string path = Path.Combine(bundlePath, bundleFolder);
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
                        if (File.Exists(file))
                        {
                            try
                            {
                                AssetBundle bundle = AssetBundle.LoadFromFile(file);
                                ValidateBundle(bundle, fileLocation, file);
                            }
                            catch (System.Exception e)
                            {
                                Debug.Log(e.ToString());
                            }
                        }
                    }
                }
            }
        }
        Digest();
    }

    private void ValidateBundle(AssetBundle bundle, string folderPath, string bundlePath)
    {
        if (bundle == null)
            return;
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
                        ARObjectScript model = new ARObjectScript();
                        model.Clone(_model);
                        model.bundlePath = bundlePath;
                        model.addr = addr;
                        model.id = bundle.name;
                        AddressBundle(model);
                    }
                }
            }
        }
    }

    private void AddressBundle(ARObjectScript model)
    {
        ARObjectModels.Add(model);
        ARObjectNames.Add(model.Name);
        ARObjectAreas.Add(model.Area);
        ARObjectBuckets.Add(model.Bucket);
        ARObjectThumbnails.Add(model.Image);
    }

    private void Digest()
    {
        ARObjectModels = ARObjectModels.Distinct().ToList();
        ARObjectNames = ARObjectNames.Distinct().ToList();
        ARObjectAreas = ARObjectAreas.Distinct().ToList();
        ARObjectBuckets = ARObjectBuckets.Distinct().ToList();
    }
}