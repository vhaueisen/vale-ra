using UnityEngine;
using UnityEngine.UI;

public class InventoryContainerComponent : MonoBehaviour
{
    private bool m_initiated = false;
    private bool m_state = true;
    public GridLayoutGroup layoutGroup;
    private bool state
    {
        get
        {
            return m_state;
        }
        set
        {
            m_state = value;
            SetState(value);
        }
    }
    public RectTransform ButtonTransform;

    public void Show()
    {
        state = true;
    }

    public void Hide()
    {
        state = false;
    }

    public void ToggleView()
    {
        state = !state;
    }

    private void SetState(bool state)
    {
        if (!m_initiated)
            Initialize();
        LerpState(state);
    }

    private void LerpState(bool state)
    {
        // if (!state)
        SetChildren(state);
        // else
        // {
        //     layoutGroup.enabled = false;
        //     SetChildren(state);
        //     layoutGroup.GetComponent<RectTransform>().LeanSize(new Vector2(938.2338f, 0.0f), 0.0f);
        //     layoutGroup.GetComponent<RectTransform>().LeanSize(new Vector2(938.2338f, 5227.272f), 1.0f).setOnComplete(
        //         () => layoutGroup.enabled = true
        //     );
        // }

    }

    private void SetChildren(bool state)
    {
        for (int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(state);
        }
    }

    private void Initialize()
    {
        ButtonTransform.localEulerAngles = Vector3.forward * (state ? 180 : 1);
        RectTransform parent = transform.parent.GetComponent<RectTransform>();
        layoutGroup.cellSize = Vector2.one * parent.rect.width * 0.475f;
        m_initiated = true;
    }
}