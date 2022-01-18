using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrainingAccordion : MonoBehaviour
{
    public Button ExpandBtn;
    public Button ConfirmBtn;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI ContentText;
    public LayoutElement layout;
    public RectTransform CheckTransform;
    private const int DefaultHeight = 130;
    private const int ExpandedHeight = 900;
    private bool isExpanded = false;
    public void Initialize(ITraining training)
    {
        ExpandBtn.onClick.AddListener(Expand);
        ConfirmBtn.onClick.AddListener(() => Trainer.InitializeTraining(training));
        Name.text = training.Name;
        ContentText.text = training.Description;
        CheckTransform.gameObject.SetActive(training.IsCompleted);
    }

    void Expand()
    {
        TweenDelta(
            isExpanded ? ExpandedHeight : DefaultHeight,
            isExpanded ? DefaultHeight : ExpandedHeight,
            0.5f, LeanTweenType.easeSpring
        );
        isExpanded = !isExpanded;
    }

    void TweenDelta(int from, int to, float duration, LeanTweenType ease)
    {
        LeanTween.value(
            layout.gameObject,
            0f,
            1f,
            duration
        ).setOnUpdate(
            (t) => layout.minHeight = (to - from) * t + from
        ).setEase(ease);
    }

}
