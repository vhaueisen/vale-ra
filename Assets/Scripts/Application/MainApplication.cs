using UnityEngine;

public class MainApplication : MonoBehaviour
{
    public SceneLoaderModel sceneLoaderModel;
    public BundleManagerModel bundleManagerModel;
    public TouchModel touchModel;
    public InventoryModel inventoryModel;
    public FooterModel footerModel;
    public CoreDataModel coreDataModel;
    public TouchView touchView;
    public FooterView footerView;
    public SceneLoaderController sceneLoaderController;

    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
