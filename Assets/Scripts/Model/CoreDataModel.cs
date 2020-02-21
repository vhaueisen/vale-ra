#pragma warning disable 0168
#pragma warning disable 0219
#pragma warning disable 0414

using System.Text;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class CoreDataModel : ApplicationElement
{
    public float ScaleSpeed = 0.25e-3f;
    public float TranslateSpeed = 4.0f;
    public float RotateSpeed = 2.0f;
    public float ElevateSpeed = 0.0001f;

    private readonly string coreDataPath = Application.persistentDataPath + "/000";
    private void Start()
    {
        string inputText = "Hello my friends!";
        byte[] inputBytes = Encoding.ASCII.GetBytes(inputText);
        byte[] encBytes;
        byte[] outputBytes;

        Debug.Log(inputText);
        encBytes = DataEncrypt.Encrypt(inputBytes, "AUAHSUAHSUHAUSHUA", "tdDf97UOFICUiu");
        Debug.Log(Encoding.ASCII.GetString(encBytes));
        outputBytes = DataEncrypt.Decrypt(encBytes, "AUAHSUAHSUHAUSHUA", "tdDf97UOFICUiu");
        Debug.Log(Encoding.ASCII.GetString(outputBytes));
    }

    private bool Load()
    {
        try
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(coreDataPath, FileMode.Open);
            byte[] dataChunk = binaryFormatter.Deserialize(fileStream) as byte[];
            fileStream.Close();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private bool Save(byte[] dataChunk)
    {
        try
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(coreDataPath, FileMode.Create);
            binaryFormatter.Serialize(fileStream, dataChunk);
            fileStream.Close();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public readonly static struct
    private bool Save(byte target)
    {
        return true;
    }
}