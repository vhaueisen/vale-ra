using UnityEngine;

public class ApplicationElement : MonoBehaviour
{
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

    public SettingsApplication SettingsApp
    {
        get
        {
            return FindObjectOfType<SettingsApplication>();
        }
    }

    public ProfileApplication ProfileApp
    {
        get
        {
            return FindObjectOfType<ProfileApplication>();
        }
    }

    public QRApplication QRApp
    {
        get
        {
            return qrApplication;
        }
    }


    private MainApplication mainApplication;
    private HomeApplication homeApplication;
    private ARApplication arApplication;
    private SettingsApplication settingsApp;
    private QRApplication qrApplication;

    void Awake()
    {
        mainApplication = GameObject.FindObjectOfType<MainApplication>();
        homeApplication = GameObject.FindObjectOfType<HomeApplication>();
        arApplication = GameObject.FindObjectOfType<ARApplication>();
        qrApplication = GameObject.FindObjectOfType<QRApplication>();
    }
}