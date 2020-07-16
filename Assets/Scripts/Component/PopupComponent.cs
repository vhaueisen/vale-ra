using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PopupComponent : MonoBehaviour
{
    public Text title;
    public Text content;
    private Text[] alphaTex;
    public Image darkOverlay;
    public GameObject container;
    public RectTransform windowRect;
    private Graphic[] graphics;
    private float[] alphas;
    private float m_initialPos;
    private const float animationSpeed = 0.15f;

    private void Awake()
    {
        alphaTex = new Text[] { title, content };
        m_initialPos = windowRect.localPosition.y;
        windowRect.LeanMoveLocalY(m_initialPos - 250.0f, 0.0f);
        graphics = GetComponentsInChildren<Graphic>(true);
        alphas = graphics.Select(o => o.color.a).ToArray();
    }

    public void Popup(string Title, string Content)
    {
        container.SetActive(true);
        title.text = Title;
        content.text = Content;
        darkOverlay.enabled = true;
        StartCoroutine(ResizeWindow());
    }

    public void Exit()
    {
        ExitTween();
    }

    private IEnumerator ResizeWindow()
    {
        yield return new WaitForEndOfFrame();
        content.text = content.text + " ";
        yield return null;
        EnterTween();
        yield break;
    }

    private void EnterTween()
    {
        windowRect.LeanMoveLocalY(m_initialPos, animationSpeed);
        TweenAlpha(true);
    }

    private void TweenAlpha(bool reverse)
    {
        for (int i = 0; i < graphics.Length; i++)
            LeanTween.alpha(graphics[i].rectTransform, reverse ? alphas[i] : 0.0f, animationSpeed).setRecursive(false);
        for (int i = 0; i < alphaTex.Length; i++)
            LeanTween.alphaText(alphaTex[i].rectTransform, reverse ? alphas[i] : 0.0f, animationSpeed).setRecursive(false);
    }

    private void ExitTween()
    {
        windowRect.LeanMoveLocalY(m_initialPos - 250.0f, animationSpeed).setOnComplete(() =>
           {
               container.SetActive(false);
               darkOverlay.enabled = false;
           }
        );
        TweenAlpha(false);
    }
}
