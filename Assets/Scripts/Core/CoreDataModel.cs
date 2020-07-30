#pragma warning disable 0168
#pragma warning disable 0219
#pragma warning disable 0414

using UnityEngine;

[System.Serializable]
public class UserSettings : DataModel
{
    [System.Serializable]
    public struct UserData
    {
        public float ScaleSpeed;
        public float TranslateSpeed;
        public float RotateSpeed;
        public float ElevateSpeed;
        public bool IsOcclusion;

        public UserData(bool initialize)
        {
            ScaleSpeed = 1.0f;
            TranslateSpeed = 1.0f;
            RotateSpeed = 1.0f;
            ElevateSpeed = 1.0f;
            IsOcclusion = false;
        }
    }
    public UserData Core = new UserData(true);
    public override void Load()
    {
        if (p_Load() && data != null)
        {
            Update();
            IsLoaded = true;
        }
    }

    public override void Update()
    {
        Core = (UserData)data;
    }
}

[System.Serializable]
public class InventorySettings : DataModel
{
    [System.Serializable]
    public struct InventoryData
    {
        public string BundlePath;
        public string Addr;
        public string Id;

    }
    public InventoryData Core = new InventoryData();
    public override void Load()
    {
        if (p_Load() && data != null)
        {
            Update();
            IsLoaded = true;
        }
    }

    public override void Update()
    {
        Core = (InventoryData)data;
    }
}


[System.Serializable]
public class ProfileSettings : DataModel
{
    [System.Serializable]
    public struct ProfileData
    {
        public int AvatarJobId;
        public int AvatarGenderId;
        public int AvatarSkinId;
        public int AvatarHairId;
    }

    public ProfileData Core = new ProfileData();
    public override void Load()
    {
        if (p_Load() && data != null)
        {
            Update();
            IsLoaded = true;
        }
    }

    public override void Update()
    {
        Core = (ProfileData)data;
    }
}

[System.Serializable]
public class LoginSettings : DataModel
{
    [System.Serializable]
    public struct AuthData
    {
        public string access_token;
        public string fullName;
        public string userName;
        public int expires_in;
        public string token_type;
        public string location;
        public string email;
        public string jobLevel;
        public string lastAuth;
    }

    public AuthData Core = new AuthData();
    public override void Load()
    {
        if (p_Load() && data != null)
        {
            Update();
            IsLoaded = true;
        }
    }

    public override void Update()
    {
        Core = (AuthData)data;
    }
}

public class CoreDataModel : ApplicationElement
{
    public UserSettings Settings;
    public InventorySettings Inventory;
    public ProfileSettings Profile;
    public LoginSettings Login;


    public virtual void Awake()
    {
        Settings = new UserSettings();
        Settings.DataName = Application.persistentDataPath + "/" + "0x000";
        Settings.DataEvent += Settings.OnSettingsEvent;
        Settings.Load();

        Inventory = new InventorySettings();
        Inventory.DataName = Application.persistentDataPath + "/" + "0x001";
        Inventory.DataEvent += Inventory.OnSettingsEvent;
        Inventory.Load();

        Profile = new ProfileSettings();
        Profile.DataName = Application.persistentDataPath + "/" + "0x002";
        Profile.DataEvent += Profile.OnSettingsEvent;
        Profile.debounceTime = 0;
        Profile.Load();

        LoadLogin();
    }

    public void LoadLogin()
    {
        Login = new LoginSettings();
        Login.DataName = Application.persistentDataPath + "/" + "0x003";
        Login.DataEvent += Login.OnSettingsEvent;
        Login.debounceTime = 0;
        Login.Load();
    }
}