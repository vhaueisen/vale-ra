using System.Collections;
using UnityEngine;

public class WindowComponent : MonoBehaviour
{
    // Start is called before the first frame update
    public RectTransform windowRect;
    public GameObject content;
    public float animationSpeed = 1.0f;
    private Vector3 defaultScale = Vector3.one;
    private Vector3 defaultPosition;
    public Transform appearFrom;
    private readonly bool playAnimation = false;
    public bool PlayAnimation
    {
        get; set;
    }
    private void Start()
    {
        if (playAnimation)
            Invoke("loadDeafult", 1.0f);
    }

    private void loadDeafult()
    {
        defaultPosition = windowRect.position;
        defaultScale = windowRect.localScale;
    }

    private float Interpolate(int framecounter)
    {
        return (float)(20.0f * framecounter * animationSpeed * Time.deltaTime);
    }

    private IEnumerator EnterAnimation()
    {
        content.SetActive(true);
        int framecounter = 0;
        while (playAnimation)
        {
            float t = Interpolate(framecounter);
            windowRect.localScale = Vector3.Lerp(Vector3.zero, defaultScale, t);
            windowRect.position = Vector3.Lerp(appearFrom.position, defaultPosition, t);
            framecounter++;
            if (t >= 1.0f)
                break;
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }

    private IEnumerator ExitAnimation()
    {
        int framecounter = 0;
        while (playAnimation)
        {
            float t = Interpolate(framecounter);
            windowRect.localScale = Vector3.Lerp(windowRect.localScale, Vector3.zero, t);
            windowRect.position = Vector3.Lerp(windowRect.position, appearFrom.position, t);
            framecounter++;
            if (t >= 1.0f)
                break;
            yield return new WaitForEndOfFrame();
        }
        content.SetActive(false);
        yield break;
    }

    public void Enter()
    {
        content.SetActive(true);
    }

    public void Exit()
    {
        content.SetActive(false);
    }
}
