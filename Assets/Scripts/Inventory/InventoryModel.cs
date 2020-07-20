#pragma warning disable 0168
#pragma warning disable 0219
#pragma warning disable 0414

using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public static string bundleFolder = "Bundles";
    private bool m_dirtyInventory;
    public static string bundlePath;
    public GameObject inventoryItem;
    public GameObject inventoryContainer;
    public List<GameObject> containerList;
    public List<InventoryContainerComponent> containerComponentList;
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
    public RectTransform loadingPanel;
    public struct ItemBucket
    {
        public ItemBucket(GameObject obj, ARObjectScript script)
        {
            Obj = obj;
            Script = script;
        }

        public GameObject Obj;
        public ARObjectScript Script;
    }
    public List<ItemBucket> bucketList;

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

    void Start()
    {
        bundlePath = Application.persistentDataPath;
        FindObjectOfType<InventoryController>().LocateBundle();
        FindObjectOfType<InventoryController>().LoadInventory();
    }
}