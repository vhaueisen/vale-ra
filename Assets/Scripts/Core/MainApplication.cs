using UnityEngine;

public class MainApplication : MonoBehaviour
{
    public SceneLoaderModel sceneLoaderModel;
    public TouchModel touchModel;
    public InventoryModel inventoryModel;
    public FooterModel footerModel;
    public CoreDataModel coreDataModel;
    public TouchView touchView;
    public FooterView footerView;
    public SceneLoaderController sceneLoaderController;
    public ToolboxController toolboxController;
    public ToolboxModel toolboxModel;
    public ToolboxView toolBoxView;
    public SlicerModel slicerModel;

    void Start()
    {
        DontDestroyOnLoad(this);
    }
}