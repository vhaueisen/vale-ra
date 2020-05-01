using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class ToolboxView : ApplicationElement
{
    private ToolboxModel model;
    private Image containerBackground;
    private float t;
    private float target;
    private Camera mainCamera;
    private Transform currentTool;
    public event EventHandler<ToolBoxEventArgs> toolBoxEvent;
    protected virtual void OnToolChange(byte toolKey)
    {
        toolBoxEvent(this, new ToolBoxEventArgs(toolKey));
    }
    private void Start()
    {
        model = MainApp.toolboxModel;
        containerBackground = model.containerTransform.GetComponent<Image>();
        currentTool = model.currentToolTransform.GetChild(0).transform;
        model.containerTransform = model.container.GetComponent<RectTransform>();
        RefreshChildren();
    }

    private void RefreshChildren()
    {
        model.openSize = model.openSize * model.containerBtnList.Length;
    }

    public void ToggleView(RectTransform toolToChange)
    {
        model.state = !model.state;
        if (model.state)
            Show();
        else
            ChangeTool(toolToChange);
    }

    private void Show()
    {
        ToggleConteinerBtn(true);
        target = model.openSize;
        StopAllCoroutines();
        StartCoroutine(LerpToTarget());
    }

    private IEnumerator LerpToTarget()
    {
        t = 0.0f;
        while (t <= 1.0f)
        {
            t += Time.deltaTime * ToolboxModel.speed;
            model.containerTransform.sizeDelta =
                new Vector2(Mathf.Lerp(model.containerTransform.sizeDelta.x,
                    target, t), model.containerTransform.sizeDelta.y);
            model.tooboxPanel.color = model.panelColor +
                new Color(0, 0, 0, (target > 0) ? (1.0f - t) : t * 0.5f);
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }

    private void Hide()
    {
        target = 0.0f;
        StopAllCoroutines();
        StartCoroutine(LerpToTarget());
        ToggleConteinerBtn(false);
    }

    private void ToggleConteinerBtn(bool b)
    {
        foreach (GameObject obj in model.containerBtnList)
            obj.SetActive(b);
        currentTool.gameObject.SetActive(true);
        if (!b)
            model.containerImgList[
                Array.IndexOf(model.containerBtnList,
                    currentTool.gameObject)].color =
                        new Color(1.0f, 1.0f, 1.0f, 0.5f);
        else
            foreach (Image i in model.containerImgList)
                i.color = Color.white;
    }

    private void ChangeTool(RectTransform toolToChange)
    {
        DecodeName(toolToChange.name);
        currentTool = model.currentToolTransform.GetChild(0);
        currentTool.SetParent(model.containerTransform);
        toolToChange.SetParent(model.currentToolTransform);
        toolToChange.anchoredPosition = Vector2.zero;
        currentTool = toolToChange.transform;
        Hide();
    }

    private void DecodeName(string name)
    {
        name = name.ToLower();
        byte newState = 0;
        if (name.Contains("animation"))
            newState = ToolBoxEventArgs.animationKey;
        else if (name.Contains("hierarchy"))
            newState = ToolBoxEventArgs.hierarchyKey;
        else if (name.Contains("slice"))
            newState = ToolBoxEventArgs.sliceKey;
        else
            newState = ToolBoxEventArgs.moveKey;

        toolBoxEvent(this, new ToolBoxEventArgs(newState));
    }

    private void Outsider()
    {
        Vector2 position = Vector3.zero;
        bool click = model.state;

        if (click)
        {
            if (Input.GetMouseButton(0))
            {
                position = Input.mousePosition;
            }
            else if (Input.touchCount > 0)
            {
                position = Input.GetTouch(0).position;
            }
            else
                click = false;
        }

        if (click && !RectTransformUtility.RectangleContainsScreenPoint(
             model.containerTransform,
             position))
        {
            model.state = !model.state;
            Hide();
        }
    }

    private void Update()
    {
        Outsider();
    }
}