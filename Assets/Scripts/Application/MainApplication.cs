using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainApplication : MonoBehaviour
{
    // ---------------------------------------------------------------------- *
    // I shall follow the rules below
    // Source of all info in the whole game. Im not destoyed on load.
    // ---------------------------------------------------------------------- *

    // References to all instances of the MVC.
    // models
    public SceneLoaderModel sceneLoaderModel;
    public BundleManagerModel bundleManagerModel;
    public TouchModel touchModel;
    public InventoryModel inventoryModel;
    public FooterModel footerModel;
    // View
    public TouchView touchView;
    public FooterView footerView;

    // Controllers
    public SceneLoaderController sceneLoaderController;
    public BundleManagerController bundleManagerController;

    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
