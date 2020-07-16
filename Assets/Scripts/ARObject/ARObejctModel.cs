#pragma warning disable 0168
#pragma warning disable 0219
#pragma warning disable 0414

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

public class ARObejctModel : ApplicationElement
{
    public string Name;
    public string Description;
    public string Area;
    public string GUID;
    public string Bucket;
    public Sprite Image;
    public bool ARImage;
    public XRReferenceImageLibrary referenceImageLibrary;
    public GameObject ARPrefab;
    public string bundlePath;
    public float InitialScaleFactor;
    public float MaxScaleFactor;
    public float MinScaleFactor;
    public float YOffset;
    public string toolbox;
    public void Clone(ARObejctModel model)
    {
        Name = model.Name;
        Description = model.Description;
        Area = model.Area;
        GUID = model.GUID;
        Bucket = model.Bucket;
        Image = model.Image;
        ARImage = model.ARImage;
        referenceImageLibrary = model.referenceImageLibrary;
        ARPrefab = model.ARPrefab;
        InitialScaleFactor = model.InitialScaleFactor;
        MaxScaleFactor = model.MaxScaleFactor;
        MinScaleFactor = model.MinScaleFactor;
        YOffset = model.YOffset;
        toolbox = model.toolbox;
    }
}
