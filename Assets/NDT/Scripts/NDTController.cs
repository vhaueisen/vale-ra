using UnityEngine;

public class NDTController : ApplicationElement
{
    public GameObject Ground;
    private bool anchored = false;
    public void Initialize()
    {
        PlaceOnPlane.OnAnchor.AddListener(onAnchor);
    }

    void onAnchor(bool b)
    {
        Ground.LeanAlpha(b ? 0.0f : 1.0f, 0.3f).setEase(b ? LeanTweenType.easeOutExpo : LeanTweenType.easeInExpo);
        anchored = b;
    }

}
