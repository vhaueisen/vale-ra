using UnityEngine.UI;
using UnityEngine;
using ZXing;
using System.Collections.Generic;
using System;
using System.Threading;

public class QRBarcodeController : MonoBehaviour
{
    private int videoWidth;
    private bool cameraAligned = false;
    public AspectRatioFitter cameraBufferFitter;
    private WebCamTexture camTexture;
    private WebCamDevice camDevice;
    private Color32[] c;
    public GameObject QRFrame;
    private bool decodeEnabled = true;
    public GameObject cameraFrame;
    public RawImage cameraBuffer;
    private bool camAvailable;
    public GameObject resultDialog;
    public Text resultText;
    private BarcodeReader barcodeReader;
    private Thread qrThread;
    private bool dialogEvent = false;
    public RectTransform imageRotator;
    private int _width;
    private int _height;
    private int offset_X;
    private int offset_Y;
    private Color32[] buffer;
    private Result decoded;
    Vector3 rotationVector = new Vector3(0f, 0f, 0f);
    Rect defaultRect = new Rect(0f, 0f, 1f, 1f);
    Rect fixedRect = new Rect(0f, 1f, 1f, -1f);
    Vector3 defaultScale = new Vector3(1f, 1f, 1f);
    Vector3 fixedScale = new Vector3(-1f, 1f, 1f);

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
            camTexture.Pause();
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
        if (WebCamTexture.devices.Length == 0)
            return;

        camDevice = WebCamTexture.devices[0];

        int w = Screen.width;
        int h = Screen.height;

        if (Screen.width / 2.0f > 900)
        {
            w = w / 2;
            h = h / 2;
        }
        camTexture = new WebCamTexture(camDevice.name, w, h - 200, 60);

        // Set camera filter modes for a smoother looking image
        camTexture.filterMode = FilterMode.Trilinear;

        cameraBuffer.texture = camTexture;

        camTexture.Play();

        camAvailable = true;

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
    }

    private void QRThread()
    {
        Thread.Sleep(2000);
        while (true)
        {
            if (decodeEnabled && c != null)
            {
                for (int i = offset_Y; i < (offset_Y + _height); i++)
                {
                    Array.Copy(c, offset_X + videoWidth * i, buffer, _width * (i - offset_Y), _width);
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
            Thread.Sleep(100);
        }
    }

    void Update()
    {
        // Skip making adjustment for incorrect camera data
        if (camTexture.width < 100 || !camAvailable)
            return;

        if (!cameraAligned)
        {
            // Rotate image to show correct orientation 
            rotationVector.z = -camTexture.videoRotationAngle;
            cameraBuffer.rectTransform.localEulerAngles = rotationVector;

            // Set AspectRatioFitter's ratio
            float videoRatio =
                (float)camTexture.width / (float)camTexture.height;
            cameraBufferFitter.aspectRatio = videoRatio;

            // Unflip if vertically flipped
            cameraBuffer.uvRect =
                camTexture.videoVerticallyMirrored ? fixedRect : defaultRect;


            cameraAligned = true;

            videoWidth = camTexture.width;
            _width = Mathf.CeilToInt((camTexture.width * (QRFrame.GetComponent<RectTransform>().rect.size.x / cameraFrame.GetComponent<RectTransform>().rect.size.x + 0.05f)));
            _height = Mathf.CeilToInt((camTexture.height * (QRFrame.GetComponent<RectTransform>().rect.size.y / cameraFrame.GetComponent<RectTransform>().rect.size.y + 0.05f)));
            offset_X = (camTexture.width - _width) / 2;
            offset_Y = (camTexture.height - _height) / 2;
            buffer = new Color32[_width * _height];
            qrThread = new Thread(QRThread);
            qrThread.Start();
        }

        if (c == null && decodeEnabled)
            c = camTexture.GetPixels32();
        if (dialogEvent)
            enterDialog(decoded.Text);
    }
}