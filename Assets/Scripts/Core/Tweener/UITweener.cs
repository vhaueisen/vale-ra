using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
public class UITweener : MonoBehaviour
{
    [SerializeField]
    private LeanTweenType m_EaseType;
    [SerializeField]
    private float m_Duration = 1f;
    [SerializeField]
    private float m_Delay = 0f;
    [Header("Animations")]
    [SerializeField]
    private TweenerParams.Move m_Move = new TweenerParams.Move();
    [SerializeField]
    private TweenerParams.Vector3 m_Rotate = new TweenerParams.Vector3();
    [SerializeField]
    private TweenerParams.Vector3 m_Scale = new TweenerParams.Vector3();
    [SerializeField]
    private TweenerParams.Color m_Color = new TweenerParams.Color();
    [SerializeField]
    private TweenerParams.Float m_Alpha = new TweenerParams.Float();
    [Header("Automatic Triggers")]
    [SerializeField]
    private bool m_HideOnAwake = true;
    [SerializeField]
    private bool m_ShowOnEnable = false;
    [SerializeField]
    private bool m_HideOnDisable = false;
    [SerializeField]
    private bool m_DestroyOnHide = false;
    [Header("Callbacks")]
    [SerializeField]
    private TweenEvent m_OnStart;
    [SerializeField]
    private TweenEvent m_OnComplete;
    [System.Serializable]
    public class TweenEvent : UnityEvent<Mode> { }
    public TweenEvent OnStart => m_OnStart;
    public TweenEvent OnComplete => m_OnComplete;
    private LTDescr tweenObject;
    private RectTransform rectTransform;
    private Graphic graphic;
    private CanvasGroup canvasGroup;

    void Attach()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        if (graphic == null)
        {
            graphic = GetComponent<Graphic>();
        }

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    void Awake()
    {
        Attach();
        if (m_HideOnAwake)
            HideImmediate();
    }

    void OnDestroy()
    {
        LeanTween.cancel(gameObject);
    }

    void OnEnable()
    {
        if (m_ShowOnEnable)
            Show();
    }

    void OnDisable()
    {
        if (m_HideOnDisable)
            Hide();
    }

    public void Show()
    {
        HandleTweens(Mode.To);
    }

    public void Hide()
    {
        HandleTweens(Mode.From);
        tweenObject.setDestroyOnComplete(m_DestroyOnHide);
    }

    public enum Mode
    {
        To,
        From,
        ToImmediate,
        FromImmediate
    }

    public void ShowImmediate() => Apply(Mode.ToImmediate);

    public void HideImmediate() => Apply(Mode.FromImmediate);

    public bool IsTweening() => LeanTween.isTweening(gameObject);

    public void HandleTweens(Mode m, bool reverse = false)
    {
        Apply(m, reverse);
        tweenObject.setDelay(m_Delay);
        tweenObject.setEase(m_EaseType);
        tweenObject.setOnStart(() => m_OnStart.Invoke(m));
        tweenObject.setOnComplete(() => m_OnComplete.Invoke(m));
    }

    private void Apply(Mode m, bool reverse = false)
    {
        float duration = m_Duration;
        if (m == Mode.FromImmediate)
        {
            m = Mode.From;
            duration = 0f;
        }
        else if (m == Mode.ToImmediate)
        {
            m = Mode.To;
            duration = 0f;
        }

        if (m_Move.enabled)
        {
            if (m == Mode.To)
            {
                rectTransform.anchoredPosition = m_Move.from.Target * (reverse ? 1f : -1f);
                tweenObject = rectTransform.LeanMove(
                    new Vector3(
                        m_Move.to.Target.x,
                        m_Move.to.Target.y,
                        rectTransform.position.z
                    ), duration);
            }
            else if (m == Mode.From)
            {
                tweenObject = rectTransform.LeanMove(
                    new Vector3(
                        m_Move.from.Target.x * (reverse ? -1f : 1f),
                        m_Move.from.Target.y * (reverse ? -1f : 1f),
                        rectTransform.position.z
                    ), duration);
            }
        }

        if (m_Rotate.enabled)
            tweenObject = LeanTween.rotate(rectTransform.gameObject, m == Mode.To ? m_Rotate.to : m_Rotate.from, duration);

        if (m_Scale.enabled)
            tweenObject = rectTransform.LeanScale(m == Mode.To ? m_Scale.to : m_Scale.from, duration);


        if (m_Color.enabled && graphic != null)
            tweenObject = LeanTween.value(
                rectTransform.gameObject,
                (Color c) => graphic.color = c,
                m == Mode.To ? m_Color.from : m_Color.to,
                m == Mode.To ? m_Color.to : m_Color.from,
                m_Duration
            );
        tweenObject = LeanTween.color(rectTransform.gameObject, m == Mode.To ? m_Color.to : m_Color.to, duration);

        if (m_Alpha.enabled)
        {
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            tweenObject = LeanTween.value(
                rectTransform.gameObject,
                (float current, float ratio) => canvasGroup.alpha = current,
                m == Mode.To ? m_Alpha.from : m_Alpha.to,
                m == Mode.To ? m_Alpha.to : m_Alpha.from,
                m_Duration
            );
        }
    }
}