using System.Collections;
using UnityEngine;

public class ToolPanelView : ApplicationElement
{
    private bool state = false;
    public Animator amin;
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
        if (projectionController != null)
            projectionController.Enabled = false;

        if (eventArgs.ToolKey == ToolBoxEventArgs.sliceKey)
        {
            ChangePanel(0);
            MainApp.slicerModel.controller.Reload();
        }
        else if (eventArgs.ToolKey == ToolBoxEventArgs.moveKey)
        {
            ToggleState(false);
            if (projectionController != null)
                projectionController.Enabled = true;
        }
        else if (eventArgs.ToolKey == ToolBoxEventArgs.hierarchyKey)
        {
            ChangePanel(1);
            MainApp.hierarchyModel.ModelXml = MainApp.hierarchyModel.socariaXml.text;
        }
        else
        {
            ToggleState(false);
        }
    }

    public void ToggleView()
    {
        state = !state;
        if (state)
            amin.Play("ToolPanelOpen");
        else
            amin.Play("ToolPanelClose");
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
                amin.Play("ToolBarToggleON");
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
        amin.Play("ToolBarToggleOFF");
        yield return new WaitForSeconds(1.0f);
        contents.SetActive(false);
        isOpen = false;
        state = false;
    }
}