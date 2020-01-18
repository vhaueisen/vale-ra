using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationElement : MonoBehaviour
{
    // ---------------------------------------------------------------------- *
    // I shall follow the rules below
    // I Gives access to the application and all mvc instances.
    // ---------------------------------------------------------------------- *
    public MainApplication MainApp
    {
        get
        {
            return mainApplication;
        }
    }

    public HomeApplication HomeApp
    {
        get
        {
            return homeApplication;
        }
    }

    public ARApplication ARApp
    {
        get
        {
            return arApplication;
        }
    }

    private MainApplication mainApplication;
    private HomeApplication homeApplication;
    private ARApplication arApplication;

    void Awake()
    {
        mainApplication = GameObject.FindObjectOfType<MainApplication>();
        homeApplication = GameObject.FindObjectOfType<HomeApplication>();
        arApplication = GameObject.FindObjectOfType<ARApplication>();
    }
}