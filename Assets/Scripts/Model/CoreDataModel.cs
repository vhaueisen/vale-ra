#pragma warning disable 0168
#pragma warning disable 0219
#pragma warning disable 0414

using System.Text;
using UnityEngine;

public class CoreDataModel : ApplicationElement
{
    public float ScaleSpeed = 0.25e-3f;
    public float TranslateSpeed = 4.0f;
    public float RotateSpeed = 2.0f;
    public float ElevateSpeed = 0.0001f;

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
}