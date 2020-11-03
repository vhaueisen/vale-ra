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
    public InventoryController inventoryController;
    public ToolboxModel toolboxModel;
    public ToolboxView toolBoxView;
    public SlicerModel slicerModel;
    public ToastNotificationComponent notificationComponent;
    public BundleManagerController bundleManager;
    public HierarchyController hierarchyController;
    public HierarchyModel hierarchyModel;
    public ARSessionController arSessionController;
    public AnimationPanelView animationPanelView;
    public AnchorController anchorController;
    void Start()
    {
        DontDestroyOnLoad(this);
    }
}