using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ARInspection;
using System;

public class InspectionItem : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    private Item item;
    private bool state = true;
    private Color[] stateColors = new Color[] { new Color(105f / 255f, 190f / 255f, 40f / 255f), new Color(187f / 255f, 19f / 255f, 62f / 255f) };
    private readonly Color defaultBgColor = new Color(0, 0, 0, 0.2f);
    public Image conformityImg;
    public Text conformityLabel;
    public Image itemBackground;
    public float durationThreshold = 1.0f;
    private bool isPointerDown = false;
    private bool longPressTriggered = false;
    private float timePressStarted;
    private static Image hintingImg;
    private static InspectionItem currentHinting;
    public static event EventHandler<InspectionEventArgs> InspectionEvent;
    public Text title;
    private void Update()
    {
        if (isPointerDown && !longPressTriggered)
        {
            itemBackground.color = new Color(
                defaultBgColor.r,
                defaultBgColor.g,
                defaultBgColor.b,
                defaultBgColor.a + 0.5f * (Time.time - timePressStarted) / durationThreshold
                );
            if (Time.time - timePressStarted > durationThreshold)
            {
                longPressTriggered = true;
                currentHinting = this;
                OnLongPress(itemBackground);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        timePressStarted = Time.time;
        isPointerDown = true;
        longPressTriggered = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!longPressTriggered)
            itemBackground.color = defaultBgColor;
        isPointerDown = false;
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        /*             itemBackground.color = defaultBgColor;
                    isPointerDown = false; */
    }

    public void Initialize(Item _item)
    {
        item = _item;
        title.text = item.Descricao;
    }

    public void OnConformityBtn()
    {
        state = !state;
        conformityImg.color = stateColors[state ? 0 : 1];
        conformityLabel.text = state ? "N" : "S";
    }

    private void OnLongPress(Image target)
    {
        if (InspectionItem.currentHinting == this)
            InspectionItem.InspectionEvent?.Invoke(this, new InspectionEventArgs(InspectionEventArgs.EventType.Unhighlight, item, () => HintUIDisable(target)));
        else
            InspectionItem.InspectionEvent?.Invoke(this, new InspectionEventArgs(InspectionEventArgs.EventType.Highlight, item, () => HintUIEnable(target)));
    }

    private void HintUIEnable(Image target)
    {
        if (InspectionItem.hintingImg != null)
        {
            InspectionItem.hintingImg.rectTransform.LeanCancel();
            InspectionItem.hintingImg.color = defaultBgColor;
        }
        target.rectTransform.LeanAlpha(0f, durationThreshold).setRecursive(false).setOnComplete(
            () => target.rectTransform.LeanAlpha(0.7f, durationThreshold).setRecursive(false).setLoopPingPong()
            );
        InspectionItem.hintingImg = target;
    }

    private void HintUIDisable(Image target)
    {
        if (InspectionItem.hintingImg != null)
        {
            InspectionItem.hintingImg.rectTransform.LeanCancel();
            InspectionItem.hintingImg.color = defaultBgColor;
        }
    }
}