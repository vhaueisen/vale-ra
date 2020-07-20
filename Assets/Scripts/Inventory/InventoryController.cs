using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static InventoryModel;

public class InventoryController : ApplicationElement
{
    private readonly byte SortByName = 0;
    private readonly byte SortByArea = 1;
    private readonly byte SortByBucket = 2;
    private readonly byte GridLayout = 0;
    private readonly byte VerticalLayout = 1;
    private int CurrentState = 0;

    public void LoadInventory()
    {
        MainApp.inventoryModel.bucketList = new List<ItemBucket>();
        foreach (Transform child in MainApp.inventoryModel.VerticalAlignedContent.transform)
            GameObject.Destroy(child.gameObject);

        foreach (Transform child in MainApp.inventoryModel.GridAlignedContent.transform)
            GameObject.Destroy(child.gameObject);

        GenerateContainers();

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
                MainApp.inventoryModel.bucketList.Add(new ItemBucket(itemInstance, model));
            }
            else
                Destroy(itemInstance);
        }
        AssetBundle.UnloadAllAssetBundles(true);
        ChangeOrder(SortByBucket);
    }

    private void GenerateContainers()
    {
        int containerSize = MainApp.inventoryModel.ARObjectBuckets.Count;
        containerSize = MainApp.inventoryModel.ARObjectAreas.Count < containerSize ? containerSize : MainApp.inventoryModel.ARObjectAreas.Count;

        GameObject[] containerList = new GameObject[containerSize];
        InventoryContainerComponent[] componentList = new InventoryContainerComponent[containerSize];

        for (int i = 0; i < containerSize; i++)
        {
            containerList[i] = Instantiate(MainApp.inventoryModel.inventoryContainer,
                Vector3.zero,
                Quaternion.identity,
                MainApp.inventoryModel.VerticalAlignedContent);
            containerList[i].SetActive(false);
            componentList[i] = containerList[i].GetComponent<InventoryContainerComponent>();
        }
        MainApp.inventoryModel.containerList = containerList.ToList();
        MainApp.inventoryModel.containerComponentList = componentList.ToList();
    }

    public void NewConteiner()
    {
        GameObject instance = Instantiate(MainApp.inventoryModel.inventoryContainer,
            Vector3.zero,
            Quaternion.identity,
            MainApp.inventoryModel.VerticalAlignedContent);
        instance.SetActive(false);
        InventoryContainerComponent instanceComponent = instance.GetComponent<InventoryContainerComponent>();
        MainApp.inventoryModel.containerList.Add(instance);
        MainApp.inventoryModel.containerComponentList.Add(instanceComponent);
    }

    private void ChangeOrder(byte mode, bool reverse = false)
    {
        CurrentState = mode;
        IEnumerable<ItemBucket> sortedList;
        if (mode == SortByName)
        {
            sortedList = MainApp.inventoryModel.bucketList.OrderBy(bucket => bucket.Script.Name);
            foreach (InventoryContainerComponent component in MainApp.inventoryModel.containerComponentList)
                component.Show();
        }
        else if (mode == SortByArea)
            sortedList = MainApp.inventoryModel.bucketList.OrderBy(bucket => bucket.Script.Area);
        else
            sortedList = MainApp.inventoryModel.bucketList.OrderBy(bucket => bucket.Script.Bucket);

        // Deactivate all containers
        foreach (GameObject container in MainApp.inventoryModel.containerList)
        {
            container.SetActive(false);
        }

        // Reverse order if necessary
        if (reverse)
            sortedList = sortedList.Reverse();

        // De-parent all items
        foreach (ItemBucket bucket in MainApp.inventoryModel.bucketList)
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

        if (mode != SortByName)
        {
            foreach (InventoryContainerComponent component in MainApp.inventoryModel.containerComponentList)
                component.Hide();
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
        CurrentState = idx;
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

    public void OnSearch(string s)
    {
        if (s.Length > 0)
        {
            ChangeOrder(SortByName);
            foreach (ItemBucket bucket in MainApp.inventoryModel.bucketList)
            {
                string tags = bucket.Script.Area + bucket.Script.Bucket + bucket.Script.Name + bucket.Script.Description;
                bucket.Obj.SetActive(tags.ToUpper().Contains(s.ToUpper()));
            }
        }
        else
        {
            foreach (ItemBucket bucket in MainApp.inventoryModel.bucketList)
            {
                bucket.Obj.SetActive(true);
                OnOrderChange(CurrentState);
            }
        }
    }

    public void AddObject(string file)
    {
        bool skip = false;
        if (MainApp.inventoryModel.containerList == null || MainApp.inventoryModel.containerList.Count == 0)
        {
            NewConteiner();
            skip = true;
        }
        MainApp.bundleManager.LoadInventoryItemData(file, skip);
        OnOrderChange(CurrentState);
        Digest();
    }

    public void ShowLoading()
    {
        MainApp.inventoryModel.loadingPanel.gameObject.SetActive(true);
        MainApp.inventoryModel.loadingPanel.LeanAlpha(0.5f, 0.5f);
    }

    public void HideLoading()
    {
        MainApp.inventoryModel.loadingPanel.LeanAlpha(0.0f, 0.5f).setOnComplete(
            () => MainApp.inventoryModel.loadingPanel.gameObject.SetActive(false)
        );
    }

    public void LocateBundle()
    {
        MainApp.bundleManager.LoadInventoryItemsData();
        Digest();
    }

    public void AddressBundle(ARObjectScript model)
    {
        MainApp.inventoryModel.ARObjectModels.Add(model);
        MainApp.inventoryModel.ARObjectNames.Add(model.Name);
        MainApp.inventoryModel.ARObjectAreas.Add(model.Area);
        MainApp.inventoryModel.ARObjectBuckets.Add(model.Bucket);
        MainApp.inventoryModel.ARObjectThumbnails.Add(model.Image);
    }

    private void Digest()
    {
        MainApp.inventoryModel.ARObjectModels = MainApp.inventoryModel.ARObjectModels.Distinct().ToList();
        MainApp.inventoryModel.ARObjectNames = MainApp.inventoryModel.ARObjectNames.Distinct().ToList();
        MainApp.inventoryModel.ARObjectAreas = MainApp.inventoryModel.ARObjectAreas.Distinct().ToList();
        MainApp.inventoryModel.ARObjectBuckets = MainApp.inventoryModel.ARObjectBuckets.Distinct().ToList();
    }
}