using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using ZXing;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine.Android;

public class QRBarcodeController : MonoBehaviour
{
    private WebCamTexture camTexture;
    private Color32[] c;
    private int W, H;
    public GameObject QRFrame;
    private bool decodeEnabled = true;
    public GameObject cameraFrame;
    private RawImage cameraBuffer;
    private bool camAvailable;
    public GameObject resultDialog;
    public Text resultText;
    private BarcodeReader barcodeReader;
    public Texture2D tex;
    private Thread qrThread;
    private bool dialogEvent = false;
    public RectTransform imageRotator;
    private int _width;
    private int _height;
    private int offset_X;
    private int offset_Y;
    private Color32[] buffer;
    private Result decoded;

    // SHIFT ALT F
    public void exitDialog()
    {
        resultDialog.SetActive(false);
        decodeEnabled = true;
    }

    private void enterDialog(string decoded)
    {
        resultText.text = decoded;
        resultDialog.SetActive(true);
        dialogEvent = false;
    }

    void OnDisable()
    {
        if (camTexture != null)
        {
            camTexture.Pause();
        }
    }

    void OnApplicationQuit()
    {
        camTexture.Stop();
        camAvailable = false;
    }

    void OnDestroy()
    {
        camTexture.Stop();
        camAvailable = false;
    }

    void Start()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            Permission.RequestUserPermission(Permission.Camera);
#endif
        cameraBuffer = cameraFrame.GetComponent<RawImage>();
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            camAvailable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing || devices.Length == 1)
            {
                camTexture = new WebCamTexture(devices[i].name, 1080, 1920, 24);
            }
        }

        if (camTexture == null)
        {
            return;
        }

        camAvailable = true;
        RectTransform cameraRect = cameraFrame.GetComponent<RectTransform>();
        camTexture.Play();
        cameraBuffer.texture = camTexture;

        // create a reader with a custom luminance source
        barcodeReader = new BarcodeReader
        {
            AutoRotate = true,
            Options = new ZXing.Common.DecodingOptions
            {
                TryHarder = true,
                PossibleFormats = new List<BarcodeFormat>
                    {
                        BarcodeFormat.QR_CODE
                    }
            }
        };

        W = camTexture.width;
        H = camTexture.height;
        float ratio = (float)W / (float)H;
        cameraFrame.GetComponent<AspectRatioFitter>().aspectRatio = ratio;
        float scaleY = camTexture.videoVerticallyMirrored ? -1f : 1f;

        cameraBuffer.rectTransform.localScale = new Vector3(1f, scaleY, 1f);
        if (Mathf.Abs(camTexture.videoRotationAngle) > 10)
        {
            imageRotator.localEulerAngles = new Vector3(0, 0, -camTexture.videoRotationAngle);
            imageRotator.localScale = Vector3.up * imageRotator.localScale.x + Vector3.right * imageRotator.localScale.y;
        }

        _width = Mathf.CeilToInt((camTexture.width * (QRFrame.GetComponent<RectTransform>().rect.size.x / cameraFrame.GetComponent<RectTransform>().rect.size.x + 0.05f)));
        _height = Mathf.CeilToInt((camTexture.height * (QRFrame.GetComponent<RectTransform>().rect.size.y / cameraFrame.GetComponent<RectTransform>().rect.size.y + 0.05f)));
        offset_X = (camTexture.width - _width) / 2;
        offset_Y = (camTexture.height - _height) / 2;
        buffer = new Color32[_width * _height];
        qrThread = new Thread(QRThread);
        qrThread.Start();
    }

    private void QRThread()
    {
        Thread.Sleep(2000);
        while (true)
        {
            if (!camAvailable)
                return;

            if (decodeEnabled && c != null)
            {
                for (int i = offset_Y; i < (offset_Y + _height); i++)
                {
                    Array.Copy(c, offset_X + W * i, buffer, _width * (i - offset_Y), _width);
                }
                // decode the current frame
                var r = barcodeReader.Decode(buffer, _width, _height);

                c = null;
                if (r != null)
                {
                    dialogEvent = true;
                    decodeEnabled = false;
                    decoded = r;
                }
            }
            Thread.Sleep(200);
        }
    }

    void Update()
    {
        if (c == null && decodeEnabled)
            c = camTexture.GetPixels32();
        if (dialogEvent)
            enterDialog(decoded.Text);
    }
}