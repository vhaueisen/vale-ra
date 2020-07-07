using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class DownloaderController : ApplicationElement
{
    private const string apiDomainURL = "https://valendo.azurewebsites.net/Vale%20RA/Models/";
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
            return (apiDomainURL + m_downloadRequestGUID + "/Model.zip");
        }
    }

    private string ThumbUrl
    {
        get
        {
            Debug.Log(apiDomainURL + m_downloadRequestGUID + "/Thumbnail.png");
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
        string[] tokenGUID = request.Split(':');
        GUID = tokenGUID[1].ToUpper();
        return (tokenGUID[0] == "ValeRA");
    }

    private IEnumerator DownloaderHandler()
    {
        string jsonResponse;
        Texture2D textureResponse;

        using (UnityWebRequest JSONRequest = UnityWebRequest.Get(JsonUrl))
        {
            yield return JSONRequest.SendWebRequest();
            if (JSONRequest.isNetworkError || JSONRequest.isHttpError)
            {
                OnRequestError();
                yield break;
            }
            jsonResponse = JSONRequest.downloadHandler.text;
        }

        using (UnityWebRequest ThumbRequest = UnityWebRequestTexture.GetTexture(ThumbUrl))
        {
            yield return ThumbRequest.SendWebRequest();
            if (ThumbRequest.isNetworkError || ThumbRequest.isHttpError)
            {
                OnRequestError();
                yield break;
            }
            DownloadHandlerTexture thumbnailHandler = ThumbRequest.downloadHandler as DownloadHandlerTexture;
            textureResponse = thumbnailHandler.texture;
        }

        OnRequestSuccess(jsonResponse, textureResponse);
    }

    private void OnRequestError()
    {
        QRApp.qrController.decodeEnabled = true;
    }

    private void OnRequestSuccess(string response, Texture2D thumbnail)
    {
        QRApp.qrController.EnterDialog(response, thumbnail);
    }
}