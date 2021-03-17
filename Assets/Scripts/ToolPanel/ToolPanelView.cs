using System.Collections;
using UnityEngine;

public class ToolPanelView : ApplicationElement
{
    private bool state = false;
    public RectTransform toolPanel;
    public GameObject[] panelList;
    private int currentPage;
    public GameObject contents;
    public GameObject root;
    private bool isOpen = false;

    private void Start()
    {
        MainApp.toolBoxView.toolBoxEvent += OnToolBoxEvent;
        MainApp.footerView.SceneLoaderEvent += OnSceneEvent;
    }

    private void OnSceneEvent(object sender, SceneLoaderEventArgs eventArgs)
    {
        root.SetActive(!(eventArgs.Scene.sceneIndex != SceneLoaderModel.HomeScene.sceneIndex && eventArgs.Scene.sceneIndex != SceneLoaderModel.ARScene.sceneIndex));
    }

    public void OnToolBoxEvent(object sender, ToolBoxEventArgs eventArgs)
    {
        ProjectionController projectionController = FindObjectOfType<ProjectionController>();
        MainApp.toolboxModel.CurrentTool = eventArgs.Key;
        MainApp.touchView.enabled = false;
        if (projectionController != null)
            projectionController.Enabled = false;

        if (eventArgs.Key == ToolBoxEventArgs.ToolKey.slice)
        {
            ChangePanel(0);
            MainApp.slicerModel.controller.Reload();
        }
        else if (eventArgs.Key == ToolBoxEventArgs.ToolKey.scaleRot)
        {
            ToggleState(false);
            MainApp.touchView.enabled = true;
            if (projectionController != null)
                projectionController.Enabled = true;
        }
        else if (eventArgs.Key == ToolBoxEventArgs.ToolKey.hierarchy)
        {
            ChangePanel(1);
            MainApp.hierarchyModel.ModelXml = MainApp.hierarchyModel.socariaXml.text;
        }
        else if (eventArgs.Key == ToolBoxEventArgs.ToolKey.animation)
        {
            MainApp.animationPanelView.initializePlayer();
            ChangePanel(2);
        }
        else if (eventArgs.Key == ToolBoxEventArgs.ToolKey.visualize)
        {
            ToggleState(false);
            MainApp.touchView.enabled = true;
        }
        else if (eventArgs.Key == ToolBoxEventArgs.ToolKey.inspection)
        {
            ChangePanel(3);
            MainApp.inspectionController.OnInspection();
            MainApp.touchView.enabled = true;
        }

        if (eventArgs.Key == ToolBoxEventArgs.ToolKey.anchor)
        {
            ToggleState(false);
            MainApp.touchView.enabled = true;
            MainApp.anchorController.Select();
            if (projectionController != null)
                projectionController.Enabled = true;
        }
        else
        {
            MainApp.anchorController.Exit();
        }

    }

    private float animationTime = 0.75f;
    private void ShowPanel()
    {
        toolPanel.LeanMoveY(700, animationTime).setEase(LeanTweenType.easeSpring);
    }

    private void HidePanel()
    {
        toolPanel.LeanMoveY(50, animationTime).setEase(LeanTweenType.easeSpring);
    }

    private void ShowToolBar()
    {
        HidePanel();
    }

    private void HideToolBar()
    {
        toolPanel.LeanMoveY(0, animationTime).setEase(LeanTweenType.easeSpring);
    }

    public void ToggleView()
    {
        state = !state;
        if (state)
            ShowPanel();
        else
            HidePanel();
    }

    public void ChangePanel(int i)
    {
        currentPage = (int)Mathf.Clamp(i, 0, panelList.Length - 1);
        ToggleState(true);
        panelList[currentPage].SetActive(true);
    }

    public void ToggleState()
    {
        state = !state;
        foreach (GameObject panel in panelList)
            panel.SetActive(false);
        contents.SetActive(state);
    }

    public void ToggleState(bool i)
    {
        if (i)
        {
            contents.SetActive(i);
            if (!isOpen)
            {
                ShowToolBar();
                isOpen = true;
            }

        }
        else
            StartCoroutine(TurnOffPanel());
        foreach (GameObject panel in panelList)
            panel.SetActive(false);
    }

    private IEnumerator TurnOffPanel()
    {
        HideToolBar();
        yield return new WaitForSeconds(1.0f);
        contents.SetActive(false);
        isOpen = false;
        state = false;
    }
}