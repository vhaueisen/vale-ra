using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
    private readonly byte SortByName = 0;
    private readonly byte SortByArea = 1;
    private readonly byte SortByBucket = 2;
    private readonly byte GridLayout = 0;
    private readonly byte VerticalLayout = 1;

    private void Start()
    {
        if (initiated)
            return;

        MainApp.inventoryModel.containerList = GenerateContainers();

        foreach (ARObjectScript model in MainApp.inventoryModel.ARObjectModels)
        {
            GameObject itemInstance = Instantiate(MainApp.inventoryModel.inventoryItem,
                Vector3.zero,
                Quaternion.identity,
                MainApp.inventoryModel.containerList[0].transform);
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
        ChangeOrder(SortByBucket);
        initiated = true;
    }

    private GameObject[] GenerateContainers()
    {
        int containerSize = MainApp.inventoryModel.ARObjectBuckets.Count;
        containerSize = MainApp.inventoryModel.ARObjectAreas.Count < containerSize ? containerSize : MainApp.inventoryModel.ARObjectAreas.Count;

        GameObject[] containerList = new GameObject[containerSize];

        for (int i = 0; i < containerSize; i++)
        {
            containerList[i] = Instantiate(MainApp.inventoryModel.inventoryContainer,
                Vector3.zero,
                Quaternion.identity,
                MainApp.inventoryModel.VerticalAlignedContent);
            containerList[i].SetActive(false);
        }

        return containerList;
    }

    private void ChangeOrder(byte mode, bool reverse = false)
    {
        IEnumerable<ItemBucket> sortedList;

        if (mode == SortByName)
            sortedList = bucketList.OrderBy(bucket => bucket.Script.Name);
        else if (mode == SortByArea)
            sortedList = bucketList.OrderBy(bucket => bucket.Script.Area);
        else
            sortedList = bucketList.OrderBy(bucket => bucket.Script.Bucket);

        // Deactivate all containers
        foreach (GameObject container in MainApp.inventoryModel.containerList)
        {
            container.SetActive(false);
        }

        // Reverse order if necessary
        if (reverse)
            sortedList = sortedList.Reverse();

        // De-parent all items
        foreach (ItemBucket bucket in bucketList)
            bucket.Obj.transform.SetParent(null);

        // Name Sort is very different from group sorting
        if (mode == SortByName)
        {
            NameSorter(sortedList);
            return;
        }

        // Group Proccess
        SwitchLayout(VerticalLayout);
        string currentListing = "";
        int containerIndex = -1;

        foreach (ItemBucket bucket in sortedList)
        {
            string containerTitle = mode == SortByArea ? bucket.Script.Area : bucket.Script.Bucket;
            if (containerTitle != currentListing)
            {
                currentListing = containerTitle;
                containerIndex++;
                MainApp.inventoryModel.containerList[containerIndex].SetActive(true);
                MainApp.inventoryModel.containerList[containerIndex].GetComponentInChildren<Text>().text = containerTitle;
            }
            bucket.Obj.transform.SetParent(MainApp.inventoryModel.containerList[containerIndex].transform);
        }
    }

    private void NameSorter(IEnumerable<ItemBucket> sortedList)
    {
        SwitchLayout(GridLayout);
        foreach (ItemBucket bucket in sortedList)
            bucket.Obj.transform.SetParent(MainApp.inventoryModel.GridAlignedContent.transform);
    }

    private void SwitchLayout(byte layoutType)
    {
        if (layoutType == GridLayout)
        {
            MainApp.inventoryModel.GridAlignedContent.gameObject.SetActive(true);
            MainApp.inventoryModel.VerticalAlignedContent.gameObject.SetActive(false);
            MainApp.inventoryModel.ScrollRect.content = MainApp.inventoryModel.GridAlignedContent.GetComponent<RectTransform>();
        }
        else
        {
            MainApp.inventoryModel.GridAlignedContent.gameObject.SetActive(false);
            MainApp.inventoryModel.VerticalAlignedContent.gameObject.SetActive(true);
            MainApp.inventoryModel.ScrollRect.content = MainApp.inventoryModel.VerticalAlignedContent.GetComponent<RectTransform>();
        }
    }

    public void OnOrderChange(int idx)
    {
        switch (idx)
        {
            case 0:
                ChangeOrder(SortByBucket);
                break;
            case 1:
                ChangeOrder(SortByName);
                break;
            case 2:
                ChangeOrder(SortByArea);
                break;
            case 3:
                ChangeOrder(SortByBucket, true);
                break;
            case 4:
                ChangeOrder(SortByName, true);
                break;
            case 5:
                ChangeOrder(SortByArea, true);
                break;
        }
    }
}