
#pragma warning disable 0162

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.Networking;
using static DownloaderModel;

public class DownloaderController : ApplicationElement
{
    private bool isEncrypted = false;
    private string EncryptionPwrd
    {
        get => "c5b0d566ee14c06429d4bd8e30260836a399977d5b693034db73662347d3d9d3";
    }
    private const string apiDomainURL = "https://gestaoconhecimento.com.br/Models/";
    private volatile bool m_downloading = false;
    private string JsonUrl
    {
        get
        {
            return (apiDomainURL + m_downloadRequestGUID + "/model.json");
        }
    }

    private string ModelUrl
    {
        get
        {
#if UNITY_EDITOR
            return (apiDomainURL + m_downloadRequestGUID + "/Model_x64.zip");
#endif
#if UNITY_IOS
            return (apiDomainURL + m_downloadRequestGUID + "/Model_IOS.zip");
#endif
            return (apiDomainURL + m_downloadRequestGUID + "/Model_Android.zip");
        }
    }

    private string ThumbUrl
    {
        get
        {
            return (apiDomainURL + m_downloadRequestGUID + "/Thumbnail.png");
        }
    }
    private string m_downloadRequestGUID;
    public string DownloadRequestGUID
    {
        //GUID is a 3-byte hexadecimal random number
        //https://www.random.org/bytes/ 
        set
        {
            string GUID;
            if (ValidGUID(value, out GUID))
            {
                m_downloadRequestGUID = GUID;
                StartCoroutine(DownloaderHandler());
            }
            else
                QRApp.qrController.decodeEnabled = true;
        }
    }

    private bool ValidGUID(string request, out string GUID)
    {
        try
        {
            string[] tokenGUID = request.Split(':');
            GUID = tokenGUID[1].ToUpper();
            return (tokenGUID[0] == "ValeRA");
        }
        catch
        {
            GUID = "";
            return false;
        }
    }

    // private void Start()
    // {
    //     MainApp.footerView.SceneLoaderEvent += OnSceneLoader;
    // }

    private IEnumerator DownloaderHandler()
    {
        DownloaderJSON jsonResponse;
        Texture2D textureResponse;

        using (UnityWebRequest JSONRequest = UnityWebRequest.Get(JsonUrl))
        {
            yield return JSONRequest.SendWebRequest();
            if (JSONRequest.result == UnityWebRequest.Result.ConnectionError
            || JSONRequest.result == UnityWebRequest.Result.DataProcessingError
            || JSONRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                OnRequestError();
                yield break;
            }
            jsonResponse = JsonUtility.FromJson<DownloaderJSON>(JSONRequest.downloadHandler.text);
        }

        using (UnityWebRequest thumbRequest = UnityWebRequestTexture.GetTexture(ThumbUrl))
        {
            yield return thumbRequest.SendWebRequest();
            if (thumbRequest.result == UnityWebRequest.Result.ConnectionError
            || thumbRequest.result == UnityWebRequest.Result.DataProcessingError
            || thumbRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                OnRequestError();
                yield break;
            }
            DownloadHandlerTexture thumbnailHandler = thumbRequest.downloadHandler as DownloadHandlerTexture;
            textureResponse = thumbnailHandler.texture;
        }
        OnRequestSuccess(jsonResponse, textureResponse);
        yield break;
    }

    private void OnRequestError()
    {
        m_downloadRequestGUID = "";
        ExitDownloadDialog();
        m_downloading = false;
    }

    private void OnRequestSuccess(DownloaderJSON response, Texture2D thumbnail)
    {
        EnterDownloadDialog();
        QRApp.downloaderModel.objectThumbnail.texture = thumbnail;
        QRApp.downloaderModel.objectName.text = response.Name;
        QRApp.downloaderModel.objectArea.text = response.Area;
        QRApp.downloaderModel.objectDescription.text = response.Description;
        isEncrypted = response.IsEncrypted;
    }
    private void EnterDownloadDialog()
    {
        QRApp.downloaderModel.downloadPanel.SetActive(true);
        QRApp.downloaderModel.downloadPanelTransform.LeanMoveY(0.0f, 0.3f);
        QRApp.downloaderModel.downloadingPanel.SetActive(false);
    }

    public void ExitDownloadDialog()
    {
        QRApp.downloaderModel.downloadPanelTransform.LeanMoveY(
            QRApp.downloaderModel.restPos, 0.3f).setOnComplete(
            () =>
            {
                QRApp.downloaderModel.downloadPanel.SetActive(false);
                QRApp.downloaderModel.downloadingPanel.SetActive(false);
            }
        );
        QRApp.qrController.decodeEnabled = true;
    }

    public void EnterDownloadingDialog()
    {
        m_downloading = true;
        bool update = Directory.Exists(
            Path.Combine(Application.persistentDataPath, "Bundles", m_downloadRequestGUID)
        );
        QRApp.downloaderModel.downloadingPanel.SetActive(true);
        QRApp.downloaderModel.downloadingPanelTransform.LeanMoveY(0.0f, 0.3f);
        QRApp.downloaderModel.downloadingPanelObjName.text = update ? string.Format(
            "Atualizando: \"{0}\"", QRApp.downloaderModel.objectName.text) :
            string.Format(
            "Baixando: \"{0}\"", QRApp.downloaderModel.objectName.text);
        StartCoroutine(DownloadAssetBundle(update));
    }

    private IEnumerator DownloadAssetBundle(bool update)
    {
        List<string> downloadedEntries = new List<string>();
        using (UnityWebRequest bundleRequest = UnityWebRequest.Get(ModelUrl))
        {
            AsyncOperation downloadOperation = bundleRequest.SendWebRequest();
            while (!downloadOperation.isDone)
            {
                QRApp.downloaderModel.progressBar.value = downloadOperation.progress;
                QRApp.downloaderModel.progressText.text = string.Format("{0:0.0}%", downloadOperation.progress * 100.0f);
                yield return null;
            }

            if (bundleRequest.result == UnityWebRequest.Result.ConnectionError
            || bundleRequest.result == UnityWebRequest.Result.DataProcessingError
            || bundleRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                OnRequestError();
                yield break;
            }

            string path = Path.Combine(Application.persistentDataPath, "Bundles", m_downloadRequestGUID);
            if (Directory.Exists(path))
                Directory.Delete(path, true);

            using (Stream data = new MemoryStream(bundleRequest.downloadHandler.data))
            {
                ZipArchive archive = new ZipArchive(data);
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    string destinationPath = Path.GetFullPath(Path.Combine(path, entry.FullName));
                    using (FileStream outputFileStream = new FileStream(destinationPath, FileMode.Create))
                        entry.Open().CopyTo(outputFileStream);
                    if (!destinationPath.Contains(".manifest"))
                    {
                        downloadedEntries.Add(destinationPath);
                    }
                }

                if (!update)
                {
                    foreach (string p in downloadedEntries)
                    {
                        try
                        {
                            MainApp.inventoryController.AddObject(p);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                QRApp.downloaderModel.downloadingPanelTransform.LeanMoveY(QRApp.downloaderModel.restPos, 0.3f).setOnComplete(
                    () =>
                    {
                        QRApp.downloaderModel.downloadingPanel.SetActive(false);
                    }
                );

                MainApp.inventoryController.ReassignConteiners();
                ExitDownloadDialog();
                MainApp.notificationComponent.Notify(string.Format("\"{0}\" adcionado ao seu inventário!", QRApp.downloaderModel.objectName.text));
                m_downloading = false;
                yield break;
            }
        }
    }

    private void OnDestroy()
    {
        if (m_downloading)
            CancelOngoingDownload();
    }

    public void CancelOngoingDownload()
    {
        StopAllCoroutines();
        string path = Path.Combine(Application.persistentDataPath, "Bundles", m_downloadRequestGUID);
        if (Directory.Exists(path))
            Directory.Delete(path, true);
        QRApp.downloaderModel.downloadingPanelTransform.LeanMoveY(QRApp.downloaderModel.restPos, 0.3f).setOnComplete(
            () =>
            {
                QRApp.downloaderModel.downloadingPanel.SetActive(false);
            }
        );
        m_downloading = false;
    }

    private void ZipTest()
    {

    }
}