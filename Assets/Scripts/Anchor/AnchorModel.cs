using UnityEngine;

public class AnchorModel : ApplicationElement
{
    public LineRenderer elevationRenderer;
    public MeshRenderer selectionRenderer;
    public float animationTime = 0.3f;
    public Transform elevationTarget;
    public bool elevating = false;
    public bool selected = false;
    public float elevateStart = 0.0f;
    public float elevateTime = 5.0f;
    public RectTransform anchorImage;
    public Camera screenCamera;
    public GameObject AnchorPrefab;
    public Transform anchorTransform;
    public volatile bool Enabled = false;
}