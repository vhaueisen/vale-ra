using UnityEngine;
using UnityEngine.XR.ARSubsystems;

public class ARObjectScript
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
    public Texture2D imageTexture;
    public string bundlePath;
    public string addr;
    public string id;
    public float InitialScaleFactor;
    public float MaxScaleFactor;
    public float MinScaleFactor;
    public float YOffset;
    public void Clone(ARObejctModel model)
    {
        Name = model.Name;
        Description = model.Description;
        Area = model.Area;
        GUID = model.GUID;
        Bucket = model.Bucket;
        if (model.Image == null)
            Debug.Log(model.Name);
        imageTexture = Texture2D.Instantiate(model.Image.texture);
        Image = Sprite.Create(imageTexture, model.Image.rect, model.Image.pivot);
        ARImage = model.ARImage;
        referenceImageLibrary = model.referenceImageLibrary;
        ARPrefab = model.ARPrefab;
        InitialScaleFactor = model.InitialScaleFactor;
        MaxScaleFactor = model.MaxScaleFactor;
        MinScaleFactor = model.MinScaleFactor;
        YOffset = model.YOffset;
    }
}