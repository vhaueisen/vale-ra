using System.Collections;
using UnityEngine;

public class ToolPanelView : ApplicationElement
{
    private bool state = false;
    private int page;
    public Animator amin;
    public GameObject[] panelList;
    private int currentPage;
    public GameObject contents;

    private void Start()
    {
        MainApp.toolBoxView.toolBoxEvent += OnToolBoxEvent;
    }
    public void Toggle(int _page)
    {
        page = _page;
    }

    public void OnToolBoxEvent(object sender, ToolBoxEventArgs eventArgs)
    {
        if (eventArgs.ToolKey == ToolBoxEventArgs.sliceKey)
        {
            if (state)
                ToggleView();
            else
            {
                ChangePanel(0);
                ToggleState(true);
                MainApp.slicerView.Reload();
            }
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
        currentPage = (int)Mathf.Clamp(0, panelList.Length - 1, currentPage);
        panelList[currentPage].SetActive(true);
    }

    public void ToggleState()
    {
        state = !state;
        foreach (GameObject panel in panelList)
            panel.SetActive(false);
        ChangePanel(currentPage);
        contents.SetActive(state);
    }

    public void ToggleState(bool i)
    {
        if (i)
        {
            contents.SetActive(i);
            amin.Play("ToolBarToggleON");
        }
        else
            StartCoroutine(TurnOffPanel());
        foreach (GameObject panel in panelList)
            panel.SetActive(false);

        ChangePanel(currentPage);
    }

    private IEnumerator TurnOffPanel()
    {
        amin.Play("ToolBarToggleOFF");
        yield return new WaitForSeconds(1.0f);
        contents.SetActive(false);
    }
}
