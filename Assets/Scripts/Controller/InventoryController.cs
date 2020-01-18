using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryController : ApplicationElement
{
    private struct ItemBucket
    {
        public ItemBucket(GameObject obj, ARObjectScript script)
        {
            Obj = obj;
            Script = script;
        }

        public GameObject Obj;
        public ARObjectScript Script;
    }
    private bool initiated = false;
    private List<ItemBucket> bucketList = new List<ItemBucket>();


    private void Start()
    {

        if (initiated)
            return;

        foreach (ARObjectScript model in MainApp.inventoryModel.ARObjectModels)
        {
            GameObject itemInstance = Instantiate(MainApp.inventoryModel.inventoryItem,
                Vector3.zero,
                Quaternion.identity,
                MainApp.inventoryModel.inventoryContent);
            InventoryItemController itemController = itemInstance.GetComponent<InventoryItemController>();

            if (itemController)
            {
                itemController.Initialize(model);
                bucketList.Add(new ItemBucket(itemInstance, model));
            }
            else
                Destroy(itemInstance);
        }
        AssetBundle.UnloadAllAssetBundles(true);
        GC.Collect();
        initiated = true;
    }

    private void ChangeOrder(IEnumerable<ItemBucket> sortedList)
    {
        foreach (ItemBucket bucket in bucketList)
        {
            bucket.Obj.transform.SetParent(null);
        }

        foreach (ItemBucket bucket in sortedList)
        {
            bucket.Obj.transform.SetParent(MainApp.inventoryModel.inventoryContent);
        }
    }

    private void OrderByName(bool desc = false)
    {
        IEnumerable<ItemBucket> sortedList = bucketList.OrderBy(bucket => bucket.Script.Name);
        if (desc)
            sortedList = sortedList.Reverse();
        ChangeOrder(sortedList);
    }

    private void OrderByArea(bool desc = false)
    {
        IEnumerable<ItemBucket> sortedList = bucketList.OrderBy(bucket => bucket.Script.Area);
        if (desc)
            sortedList = sortedList.Reverse();
        ChangeOrder(sortedList);
    }

    private void OrderByBucket(bool desc = false)
    {
        IEnumerable<ItemBucket> sortedList = bucketList.OrderBy(bucket => bucket.Script.Bucket);
        if (desc)
            sortedList = sortedList.Reverse();
        ChangeOrder(sortedList);
    }

    public void OnOrderChange(int idx)
    {
        switch (idx)
        {
            case 0:
                OrderByBucket();
                break;
            case 1:
                OrderByName();
                break;
            case 2:
                OrderByArea();
                break;
            case 3:
                OrderByBucket(true);
                break;
            case 4:
                OrderByName(true);
                break;
            case 5:
                OrderByArea(true);
                break;
        }
    }
}