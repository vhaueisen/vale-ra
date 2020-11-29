using UnityEngine;
using UnityEditor.Media;
using Unity.Collections;
using System.IO;
using System.Collections;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

public class ARCameraRecorderController : MonoBehaviour
{
    private int width;
    private int height;
    volatile bool screenshot = false;
    volatile bool record = false;
    private const int bufferSize = 256;
    private Color[][][] videoBuffer = new Color[2][][];
    private volatile int videoBufferIdx = 0;
    private Texture2D render;
    private Texture2D videoOutput;
    private AudioTrackAttributes audioAttr;
    private VideoTrackAttributes videoAttr;
    private int sampleFramesPerVideoFrame;
    private float framerate = 30f;
    private int frameBuffer = 0;
    private int m_toDump = -1;
    private int toDump
    {
        get => (int)(m_toDump * 0.5f + 0.5f);
        set => m_toDump = value;
    }
    private int m_toWrite = -1;
    private int toWrite
    {
        get => (int)(m_toWrite * 0.5f + 0.5f);
        set => m_toWrite = value;
    }
    private string folderPath = @"C:\Users\vitor\Desktop\Vale RA\Captures\";
    private string videoPath = "capture.mp4";
    private string binPath = "bin_";

    private Color[] MirrorY(Color[] image)
    {
        Color[] mirrored = new Color[image.Length];

        for (int i = 0; i < image.Length / width; i++)
        {
            System.Array.Copy(image, mirrored.Length - (i + 1) * width, mirrored, i * width, width);
        }
        return mirrored;
    }
    private void DisposeVideo()
    {
        Debug.Log("Disposing...");
        BinaryFormatter formatter = new BinaryFormatter();
        using (var encoder = new MediaEncoder(videoPath, videoAttr, audioAttr))
        {
            using (var audioBuffer = new NativeArray<float>(sampleFramesPerVideoFrame, Allocator.Temp))
            {
                for (int i = 0; i < frameBuffer; i++)
                {
                    using (FileStream fs = new FileStream(binPath + i, FileMode.Open))
                    {
                        Color[][] frames = FloatToColor((float[][])formatter.Deserialize(fs));
                        for (int j = 0; j < frames.Length; j++)
                        {
                            videoOutput.SetPixels(MirrorY(frames[j]));
                            videoOutput.Apply();
                            encoder.AddFrame(videoOutput);
                            encoder.AddSamples(audioBuffer);
                        }
                    }
                }
            }
        }
    }

    private float[][] ColorToFloat(Color[][] color)
    {
        float[][] result = new float[color.Length][];
        for (int i = 0; i < color.Length; i++)
        {
            result[i] = new float[3 * width * height];
            int j = 0;
            int k = 0;
            while (j < 3 * width * height)
            {
                result[i][j] = color[i][k].r;
                j++;
                result[i][j] = color[i][k].g;
                j++;
                result[i][j] = color[i][k].b;
                j++;
                k++;
            }
        }
        return result;
    }

    private Color[][] FloatToColor(float[][] floats)
    {
        Color[][] result = new Color[floats.Length][];
        for (int i = 0; i < floats.Length; i++)
        {
            result[i] = new Color[width * height];
            int j = 0;
            int k = 0;
            while (j < width * height)
            {
                result[i][j] = new Color(floats[j][k], floats[j][k + 1], floats[j][k + 2]);
                j++;
                k = k + 3;
            }
        }
        return result;
    }
    public void StartRecording()
    {
        record = true;
        videoBufferIdx = 0;
        StartCoroutine(RecordMovieAsync());

    }

    public void StopRecording()
    {
        record = false;
    }

    private IEnumerator RecordMovieAsync()
    {
        Debug.Log("Recording Started.");
        while (record)
        {
            for (int i = 0; i < bufferSize; i++)
            {
                yield return new WaitForSeconds(1.0f / framerate);
                yield return new WaitForEndOfFrame();
                var rt = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
                ScreenCapture.CaptureScreenshotIntoRenderTexture(rt);
                AsyncGPUReadback.Request(rt, 0, TextureFormat.ARGB32, delegate (AsyncGPUReadbackRequest request) { OnCompleteReadback(request); });
                RenderTexture.ReleaseTemporary(rt);
                if (!record)
                    break;
            }
            toWrite = toWrite * -1;
            bufferDump();
        }
        DisposeVideo();
        Debug.Log("Recording Finished.");
        yield break;
    }

    void OnCompleteReadback(AsyncGPUReadbackRequest request)
    {
        if (request.hasError)
        {
            Debug.Log("GPU readback error detected.");
            return;
        }

        render.LoadRawTextureData(request.GetData<uint>());
        render.Apply();
        videoBuffer[toWrite][videoBufferIdx] = render.GetPixels();
        videoBufferIdx++;
    }

    private int framecount = 0;
    private void bufferDump()
    {
        videoBufferIdx = 0;
        framecount++;
        StartCoroutine(DumpAsync());
    }

    private IEnumerator DumpAsync()
    {
        using (FileStream fs = new FileStream(binPath + frameBuffer, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, ColorToFloat(videoBuffer[toDump]));
            frameBuffer++;
        }
        toDump = toDump * -1;
        if (framecount == 1)
            StopRecording();
        yield break;
    }



    private void Awake()
    {
        width = Screen.width;
        height = Screen.height;
        render = new Texture2D(width, height, TextureFormat.ARGB32, false);
        videoOutput = new Texture2D(width, height, TextureFormat.RGBA32, false);
        videoAttr = new VideoTrackAttributes
        {
            // 29.97 FPS
            frameRate = new MediaRational(30),
            width = (uint)width,
            height = (uint)height,
            includeAlpha = false
        };

        audioAttr = new AudioTrackAttributes
        {
            sampleRate = new MediaRational(48000),
            channelCount = 2,
            language = "fr"
        };
        sampleFramesPerVideoFrame = audioAttr.channelCount *
            audioAttr.sampleRate.numerator / videoAttr.frameRate.numerator;
        videoPath = folderPath + videoPath;
        binPath = folderPath + binPath;
        videoBuffer[0] = new Color[bufferSize][];
        videoBuffer[1] = new Color[bufferSize][];
        for (int i = 0; i < 2; i++)
            for (int j = 0; j < bufferSize; j++)
            {
                videoBuffer[i][j] = new Color[width * height];
            }
    }
}