using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WindowComponent : MonoBehaviour
{
    public ScrollRect scrollRect;
    public bool state = false;
    public RectTransform windowRect;
    public GameObject content;
    private const float animationSpeed = 0.15f;
    private float m_initialPos;
    public Graphic[] graphics;
    public float[] alphas;
    private const float shakeSpeed = 0.2f;
    private const float shakeAmount = 0.05f;
    public enum AnimationType
    {
        SlideFade,
        Shake,
        None
    };
    public AnimationType animationType = AnimationType.SlideFade;
    private void Awake()
    {
        m_initialPos = windowRect.localPosition.y;
        if (animationType == AnimationType.SlideFade)
        {
            windowRect.LeanMoveLocalY(m_initialPos - 250.0f, 0.0f);
            graphics = GetComponentsInChildren<Graphic>(true);
            alphas = graphics.Select(o => o.color.a).ToArray();
        }
    }

    public void Enter()
    {
        scrollRect.verticalNormalizedPosition = 1.0f;
        state = true;
        content.SetActive(true);
        EnterTween();
    }

    public void Exit()
    {
        state = false;
        ExitTween();
    }

    public void Toggle()
    {
        state = !state;
        if (state)
            Enter();
        else
            Exit();
    }

    private void EnterTween()
    {
        if (animationType == AnimationType.SlideFade)
        {
            windowRect.LeanMoveLocalY(m_initialPos, animationSpeed);
            TweenAlpha(true);
        }
        else if (animationType == AnimationType.Shake)
        {
            windowRect.localScale = Vector3.one * (1.0f + shakeAmount);
            windowRect.LeanScale(Vector3.one, shakeSpeed).setEase(LeanTweenType.easeSpring);
        }
    }

    private void TweenAlpha(bool reverse)
    {
        for (int i = 0; i < graphics.Length; i++)
            LeanTween.alpha(graphics[i].rectTransform, reverse ? alphas[i] : 0.0f, animationSpeed).setRecursive(false);
    }

    private void ExitTween()
    {
        if (animationType == AnimationType.SlideFade)
        {
            windowRect.LeanMoveLocalY(m_initialPos - 250.0f, animationSpeed).setOnComplete
            (
                () => content.SetActive(false)
            );
            TweenAlpha(false);
        }
        else if (animationType == AnimationType.Shake)
        {
            windowRect.localScale = Vector3.one * (1.0f - shakeAmount);
            windowRect.LeanScale(Vector3.one, shakeSpeed).setEase(LeanTweenType.easeSpring).setOnComplete
            (
                () => content.SetActive(false)
            );
        }
        else
        {
            content.SetActive(false);
        }
    }
}