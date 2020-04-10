using UnityEngine;
using System.Collections;

public class ToolboxView : ApplicationElement
{
    public ToolboxModel model;
    private float t;
    private float target;

    private void Start()
    {
        model = MainApp.toolboxModel;
    }
    public void ToggleView()
    {
        model.state = !model.state;
        if (model.state)
            Show();
        else
            Hide();
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
            model.containerTransform.sizeDelta = new Vector2(Mathf.Lerp(model.containerTransform.sizeDelta.x, target, t), model.containerTransform.sizeDelta.y);
            model.tooboxPanel.color = model.panelColor + new Color(0, 0, 0, (target > 0) ? (1.0f - t) : t * 0.5f);
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
    }
}