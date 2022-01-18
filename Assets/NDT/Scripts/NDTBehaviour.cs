using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR.ARFoundation;

public class NDTBehaviour : ApplicationElement
{
    public Button backBtn;
    public Button anchorBtn;
    const float d = 1f;
    CanvasGroup staticUI;
    public CanvasGroup tutorialUI;
    public static NDTBehaviour Instance;
    void Start()
    {
        Instance = this;
        tutorialUI.LeanAlpha(1f, 0.5f).setEase(LeanTweenType.easeInExpo);
        staticUI = GameObject.FindWithTag("StaticUI").GetComponent<CanvasGroup>();
        VideoPlayer.enabled = true;
        VideoPlayer.Play();
        _rawImageTexture = RawImage.texture;
        VideoPlayer.prepareCompleted += PrepareCompleted;
        Initialize();
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

    public void Back()
    {
        staticUI.gameObject.SetActive(true);
        FooterView.OnSceneLoader(SceneLoaderModel.LearningScene);
        staticUI.LeanAlpha(1f, 0.5f).setEase(LeanTweenType.easeInExpo);
        backBtn.gameObject.LeanScale(Vector3.zero, 0.5f).setEase(LeanTweenType.easeSpring).setOnComplete(
            () => backBtn.gameObject.SetActive(false)
        );
    }

    public void OnPlanes()
    {
        placeOnPlane.enabled = true;
        anchorBtn.onClick.AddListener(placeOnPlane.ToggleAnchor);
        anchorBtn.gameObject.LeanScale(Vector3.one, 0.5f).setEase(LeanTweenType.easeSpring);
        tutorialUI.LeanAlpha(0f, 0.5f).setEase(LeanTweenType.easeOutExpo).setOnComplete(() => tutorialUI.gameObject.SetActive(false));
    }

    public PlaceOnPlane placeOnPlane;
    public RawImage RawImage;
    public VideoPlayer VideoPlayer;
    private Texture _rawImageTexture;
    public ARPlaneManager manager;
    public int targetPlanes = 5;
    private bool foundPlanes { get => manager.trackables.count >= targetPlanes; }
    public void Update()
    {
        anchorBtn.interactable = !InteractableObject.IsInretacting;
        if (RawImage.enabled && foundPlanes)
        {
            VideoPlayer.Stop();
            RawImage.texture = _rawImageTexture;
            VideoPlayer.enabled = false;
            RawImage.enabled = false;
            OnPlanes();
        }
    }

    private void PrepareCompleted(VideoPlayer player)
    {
        RawImage.texture = player.texture;
    }
}
