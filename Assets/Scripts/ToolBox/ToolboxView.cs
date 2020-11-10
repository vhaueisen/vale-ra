using UnityEngine;
using UnityEngine.UI;
using System;
using static ToolboxModel;

public class ToolboxView : ApplicationElement
{
    private ToolboxModel model;
    private Image containerBackground;
    private float t;
    private float target;
    private Camera mainCamera;
    private RectTransform currentTool;
    public event EventHandler<ToolBoxEventArgs> toolBoxEvent;
    private byte m_currentState = ToolBoxEventArgs.anchorKey;
    private float m_offset = 600.0f;
    protected virtual void OnToolChange(byte toolKey)
    {
        toolBoxEvent(this, new ToolBoxEventArgs(toolKey));
    }

    public void ChangeObject(ARObejctModel script)
    {
        if (m_currentState != ToolBoxEventArgs.anchorKey)
            ChangeTool(model.anchorBtn);
        Toolbox tools = new Toolbox(script.toolbox);
        ReArangeBtns(tools);
    }

    private void ReArangeBtns(Toolbox tools)
    {
        GameObject[] btns = new GameObject[]
        {
            model.sliceBtn.gameObject,
            model.hierarchyBtn.gameObject,
            model.animationBtn.gameObject
        };
        m_offset = 600;
        for (int i = 0; i < btns.Length; i++)
        {
            btns[i].SetActive(tools.btnStates[i]);
            if (tools.btnStates[i])
                m_offset = m_offset + 200;
        }
        Vector2 offset = new Vector2(m_offset, 175);
        model.btnPanel.sizeDelta = offset;
        model.btnContainer.sizeDelta = offset;
    }

    private void Start()
    {
        model = MainApp.toolboxModel;
    }

    public void ToggleView(RectTransform toolToChange)
    {
        model.state = !model.state;
        if (model.state)
            Show();
        else
            ChangeTool(toolToChange);
    }

    public void OnSceneLoad()
    {
        if (m_currentState != ToolBoxEventArgs.anchorKey)
            ChangeTool(model.anchorBtn);
    }
    private float m_animationSpeed = 0.3f;

    private void CancelTweens()
    {
        model.toolRectangle.LeanCancel();
        model.btnPanel.LeanCancel();
        model.btnContainer.LeanCancel();
        if (currentTool != null)
            currentTool.LeanCancel();
    }
    private void Show()
    {
        CancelTweens();
        model.toolRectangle.LeanAlpha(0.0f, m_animationSpeed);
        model.toolboxMask.enabled = false;
        model.btnPanel.LeanAlpha(
            0.5f, 1.3f * m_animationSpeed
        ).setRecursive(false);
        model.btnPanel.LeanMoveX(
            0.0f, m_animationSpeed
        );
        model.btnContainer.LeanAlpha(
            1.0f, m_animationSpeed
        );
    }

    private void Hide()
    {
        CancelTweens();
        model.toolRectangle.LeanAlpha(0.5f, m_animationSpeed);
        model.btnPanel.LeanAlpha(
            0.0f, 1.3f * m_animationSpeed
        ).setRecursive(false).setOnComplete(
            () =>
            {
                model.toolboxMask.enabled = true;
                model.toolRectangle.gameObject.SetActive(true);
            }
        );
        model.btnPanel.LeanMoveX(
            m_offset - 150 - currentTool.anchoredPosition.x, m_animationSpeed
        );
        model.btnContainer.LeanAlpha(
            0.0f, m_animationSpeed
        );
        currentTool.LeanCancel();
        currentTool.LeanAlpha(
            0.75f, m_animationSpeed
        );
    }

    private void ChangeTool(RectTransform toolToChange)
    {
        DecodeName(toolToChange.name);
        currentTool = toolToChange;
        Hide();
    }

    private void DecodeName(string name)
    {
        name = name.ToLower();
        if (name.Contains("animation"))
            m_currentState = ToolBoxEventArgs.animationKey;
        else if (name.Contains("hierarchy"))
            m_currentState = ToolBoxEventArgs.hierarchyKey;
        else if (name.Contains("slice"))
            m_currentState = ToolBoxEventArgs.sliceKey;
        else if (name.Contains("size"))
            m_currentState = ToolBoxEventArgs.scaleRotKey;
        else if (name.Contains("anchor"))
            m_currentState = ToolBoxEventArgs.anchorKey;
        else if (name.Contains("visualize"))
            m_currentState = ToolBoxEventArgs.visualizeKey;
        toolBoxEvent(this, new ToolBoxEventArgs(m_currentState));
    }

    private void Outsider()
    {
        Vector2 position = Vector3.zero;
        bool click = model.state;

        if (click)
        {
#if UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                position = Input.mousePosition;
            }
#endif
            if (Input.touchCount > 0)
            {
                position = Input.GetTouch(0).position;
            }
            else
                click = false;
        }

        if (click && !RectTransformUtility.RectangleContainsScreenPoint(
             model.btnContainer, position))
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