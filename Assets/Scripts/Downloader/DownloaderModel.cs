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
    }

    public Slider progressBar;
    public Text objectName;
    public Text progressText;
    public Text objectDescription;
    public RawImage objectThumbnail;
    public GameObject downloadPanel;
    public GameObject downloadingPanel;
}