using UnityEngine;
using UnityEngine.UI;

public class DownloaderModel : ApplicationElement
{
    public struct DownloaderJSON
    {
        public string Name;
        public string Description;
        public string Area;
        public string Bucket;
        public string GUID;
        public bool IsEncrypted;
    }

    public Slider progressBar;
    public Text objectName;
    public Text objectArea;
    public Text progressText;
    public Text downloadingPanelObjName;
    public Text objectDescription;
    public RawImage objectThumbnail;
    public GameObject downloadPanel;
    public GameObject downloadingPanel;
    public RectTransform downloadPanelTransform;
    public RectTransform downloadingPanelTransform;
    public float restPos;
    private void Awake()
    {
        restPos = downloadPanelTransform.position.y;
    }
}