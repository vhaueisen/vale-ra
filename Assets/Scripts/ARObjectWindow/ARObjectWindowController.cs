using UnityEngine.UI;
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
        MainApp.bundleManager.LoadObject(tempModel);
        UpdateVars();
    }
}