#pragma warning disable 0168
#pragma warning disable 0219
#pragma warning disable 0414

using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class DataModel
{
    public object data;

    public string DataName
    {
        get
        {
            return DataName;
        }
        set
        {
            DataName = Application.persistentDataPath + "/" + value;
        }
    }

    private protected bool Save()
    {
        try
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(DataName, FileMode.Create);
            binaryFormatter.Serialize(fileStream, data);
            fileStream.Close();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private protected bool Load()
    {
        try
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(DataName, FileMode.Open);
            data = binaryFormatter.Deserialize(fileStream) as byte[];
            fileStream.Close();
            return true;
        }
        catch
        {
            return false;
        }
    }
}


public class UserSettings : DataModel
{
    public float ScaleSpeed;
    public float TranslateSpeed;
    public float RotateSpeed;
    public float ElevateSpeed;
    public bool IsOcclusion;
    public UserSettings(float scaleSpeed, float translateSpeed,
        float rotateSpeed, float elevateSpeed, bool isOcclusion)
    {
        ScaleSpeed = scaleSpeed;
        TranslateSpeed = translateSpeed;
        RotateSpeed = rotateSpeed;
        ElevateSpeed = elevateSpeed;
        IsOcclusion = isOcclusion;
    }

    new public bool Save()
    {
        Load();
        return true;
    }
}


public class CoreDataModel : ApplicationElement
{
    void Start()
    {

    }
}