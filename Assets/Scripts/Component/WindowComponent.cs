using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WindowComponent : MonoBehaviour
{
    public RectTransform windowRect;
    public GameObject content;
    private const float animationSpeed = 0.15f;
    public bool animated = true;
    private float m_initialPos;
    public Graphic[] graphics;
    public float[] alphas;

    private void Awake()
    {
        m_initialPos = windowRect.localPosition.y;
        if (animated)
        {
            windowRect.LeanMoveLocalY(m_initialPos - 250.0f, 0.0f);
            graphics = GetComponentsInChildren<Graphic>(true);
            alphas = graphics.Select(o => o.color.a).ToArray();
        }
    }

    private void Start()
    {
        //     for (int i = 0; i < graphics.Length; i++)
        //         LeanTween.alpha(graphics[i].rectTransform, 0.0f, 0.0f)
        //             .setRecursive(false)
        //                 .setOnComplete(
        //                     () => content.SetActive(false));
    }

    public void Enter()
    {
        content.SetActive(true);
        EnterTween();
    }

    public void Exit()
    {
        ExitTween();
    }

    private void EnterTween()
    {
        if (!animated)
            return;
        windowRect.LeanMoveLocalY(m_initialPos, animationSpeed);
        TweenAlpha(true);
    }

    private void TweenAlpha(bool reverse)
    {
        for (int i = 0; i < graphics.Length; i++)
            LeanTween.alpha(graphics[i].rectTransform, reverse ? alphas[i] : 0.0f, animationSpeed).setRecursive(false);
    }

    private void ExitTween()
    {
        if (!animated)
        {
            content.SetActive(false);
            return;
        }
        else
        {
            windowRect.LeanMoveLocalY(m_initialPos - 250.0f, animationSpeed).setOnComplete(
                () => content.SetActive(false));
            TweenAlpha(false);
        }
    }
}