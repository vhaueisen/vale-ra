using UnityEngine;
using ZXing;
using System.Collections.Generic;
using System;
using System.Threading;

public class QRBarcodeController : ApplicationElement
{
    private int videoWidth;
    private bool cameraAligned = false;
    private WebCamTexture camTexture;
    private WebCamDevice camDevice;
    private Color32[] c;
    public bool decodeEnabled = true;
    private bool camAvailable;
    private BarcodeReader barcodeReader;
    private Thread qrThread;
    private bool dialogEvent = false;
    private int _width;
    private int _height;
    private int offset_X;
    private int offset_Y;
    private Color32[] buffer;
    private Result decoded;
    private Vector3 rotationVector = new Vector3(0f, 0f, 0f);
    private Rect defaultRect = new Rect(0f, 0f, 1f, 1f);
    private Rect fixedRect = new Rect(0f, 1f, 1f, -1f);
    private Vector3 defaultScale = new Vector3(1f, 1f, 1f);
    private Vector3 fixedScale = new Vector3(-1f, 1f, 1f);
    private QRBarcodeModel model;
    private volatile bool m_enabled;

    void OnDisable()
    {
        if (camTexture != null)
            camTexture.Pause();
    }

    void OnApplicationQuit()
    {
        camAvailable = false;
        m_enabled = false;
        if (camTexture != null)
            camTexture.Stop();
    }

    void OnDestroy()
    {
        camAvailable = false;
        m_enabled = false;
        if (camTexture != null)
            camTexture.Stop();
    }

    void Start()
    {
        model = QRApp.qrModel;
        if (WebCamTexture.devices.Length == 0)
            return;

        camDevice = WebCamTexture.devices[0];

        camTexture = new WebCamTexture(camDevice.name, 1280, 720, 30);
        if (camTexture == null)
            return;
        // Set camera filter modes for a smoother looking image
        camTexture.filterMode = FilterMode.Trilinear;

        model.cameraBuffer.texture = camTexture;

        camTexture.Play();

        camAvailable = true;

        // create a reader with a custom luminance source
        barcodeReader = new BarcodeReader
        {
            AutoRotate = true,
            TryInverted = true,
            Options = new ZXing.Common.DecodingOptions
            {

                PossibleFormats = new List<BarcodeFormat>
                    {
                        BarcodeFormat.QR_CODE
                    }
            }
        };
        m_enabled = true;
    }

    private void QRThread()
    {
        while (m_enabled)
        {
            if (decodeEnabled && c != null)
            {
                for (int i = offset_Y; i < (offset_Y + _height); i++)
                {
                    Array.Copy(c, offset_X + videoWidth * i, buffer, _width * (i - offset_Y), _width);
                }
                Result r = barcodeReader.Decode(buffer, _width, _height);
                c = null;
                if (r != null)
                {
                    dialogEvent = true;
                    decodeEnabled = false;
                    decoded = r;
                }
            }
            Thread.Sleep(250);
        }
    }

    void Update()
    {
        if (m_enabled)
        {
            if (camTexture.width < 100 || !camAvailable)
                return;

            if (!cameraAligned)
            {
                rotationVector.z = -camTexture.videoRotationAngle;
                model.cameraBuffer.rectTransform.localEulerAngles = rotationVector;
                float videoRatio =
                    (float)camTexture.width / (float)camTexture.height;
                model.cameraBufferFitter.aspectRatio = videoRatio;
                model.cameraBuffer.uvRect =
                    camTexture.videoVerticallyMirrored ? fixedRect : defaultRect;
                cameraAligned = true;
                videoWidth = camTexture.width;
                _width = Mathf.CeilToInt((camTexture.width * (model.qrFrameRect.rect.size.x / model.cameraFrameRect.rect.size.x)));
                _height = Mathf.CeilToInt((camTexture.height * (model.qrFrameRect.rect.size.y / model.cameraFrameRect.rect.size.y)));
                offset_X = (camTexture.width - _width) / 2;
                offset_Y = (camTexture.height - _height) / 2;
                buffer = new Color32[_width * _height];
                qrThread = new Thread(QRThread);
                qrThread.Start();
            }

            if (c == null && decodeEnabled)
                c = camTexture.GetPixels32();
            if (dialogEvent)
            {
                QRApp.downloaderController.DownloadRequestGUID = decoded.Text;
                dialogEvent = false;
            }
        }
    }
}