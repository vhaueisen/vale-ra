#pragma warning disable 0168
#pragma warning disable 0219
#pragma warning disable 0414

using UnityEngine;

[System.Serializable]
public class UserSettings : DataModel
{
    [System.Serializable]
    public struct CoreData
    {
        public float ScaleSpeed;
        public float TranslateSpeed;
        public float RotateSpeed;
        public float ElevateSpeed;
        public bool IsOcclusion;
    }
    public CoreData Core = new CoreData();

    public UserSettings(string name)
    {
        DataName = name;
    }

    public void Load()
    {
        if (p_Load() && data != null)
        {
            Core = (CoreData)data;
            IsLoaded = true;
        }
    }

    public void OnSettingsEvent(object sender, DataEventArgs dataEventArgs)
    {
        Debug.Log("Event Fired!");
        if (dataEventArgs.EventType == DataEventArgs.LoadEvent)
        {
            Load();
        }
        else if (dataEventArgs.EventType == DataEventArgs.SaveEvent)
        {
            p_Save();
        }
        else
        {
            p_Update(dataEventArgs.DataChunk);
        }
    }
}


public class CoreDataModel : ApplicationElement
{
    public UserSettings Settings;

    void Start()
    {
        Settings = new UserSettings(Application.persistentDataPath + "/" + "data.001");
        Settings.DataEvent += Settings.OnSettingsEvent;

        Settings.Load();
    }
}