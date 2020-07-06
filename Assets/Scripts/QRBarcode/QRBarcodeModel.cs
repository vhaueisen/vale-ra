using UnityEngine;
using UnityEngine.UI;

public class QRBarcodeModel : ApplicationElement
{
    public AspectRatioFitter cameraBufferFitter;
    public GameObject QRFrame;
    public GameObject cameraFrame;
    public RawImage cameraBuffer;
    public GameObject resultDialog;
    public Text resultText;
    public RectTransform imageRotator;
    public RectTransform qrFrameRect;
    public RectTransform cameraFrameRect;
}
