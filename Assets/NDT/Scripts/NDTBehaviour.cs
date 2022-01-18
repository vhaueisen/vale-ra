using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR.ARFoundation;

public class NDTBehaviour : ApplicationElement
{
    public Button backBtn;
    const float d = 2f;
    private bool _anchor = false;
    CanvasGroup staticUI;
    CanvasGroup tutorialUI;

    private bool anchor
    {
        set
        {
            if (value && !_anchor)
                Initialize();
            _anchor = value;
        }
    }

    void Start()
    {
        PlaceOnPlane.OnAnchor.AddListener((b) => anchor = b);
        staticUI = GameObject.FindWithTag("StaticUI").GetComponent<CanvasGroup>();
        VideoPlayer.enabled = true;
        VideoPlayer.Play();
        _rawImageTexture = RawImage.texture;
        VideoPlayer.prepareCompleted += PrepareCompleted;
    }

    public void Initialize()
    {
        staticUI.LeanAlpha(0f, 0.5f).setDelay(d).setEase(LeanTweenType.easeOutExpo).setOnComplete(
            () => staticUI.gameObject.SetActive(false)
        );
        backBtn.gameObject.SetActive(true);
        backBtn.gameObject.LeanScale(Vector3.one, 0.5f).setDelay(d).setEase(LeanTweenType.easeSpring);
        backBtn.onClick.AddListener(Back);
    }

    void Back()
    {
        staticUI.gameObject.SetActive(true);
        MainApp.footerView.LoadHomeScene();
        staticUI.LeanAlpha(1f, 0.5f).setEase(LeanTweenType.easeInExpo);
        backBtn.gameObject.LeanScale(Vector3.zero, 0.5f).setEase(LeanTweenType.easeSpring).setOnComplete(
            () => backBtn.gameObject.SetActive(false)
        );
    }

    public void OnPlanes()
    {
        placeOnPlane.enabled = true;
    }

    public PlaceOnPlane placeOnPlane;
    public RawImage RawImage;
    public VideoPlayer VideoPlayer;
    private Texture _rawImageTexture;
    public ARPlaneManager manager;
    public int targetPlanes = 3;
    private bool foundPlanes { get => manager.trackables.count >= targetPlanes; }
    public void Update()
    {
        if (RawImage.enabled && foundPlanes)
        {
            VideoPlayer.Stop();
            RawImage.texture = _rawImageTexture;
            VideoPlayer.enabled = false;
            RawImage.enabled = false;
            tutorialUI.LeanAlpha(0f, 0.3f).setEase(LeanTweenType.easeOutExpo);
        }
    }

    private void PrepareCompleted(VideoPlayer player)
    {
        RawImage.texture = player.texture;
    }
}
