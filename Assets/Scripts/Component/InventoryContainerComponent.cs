using UnityEngine;
using UnityEngine.UI;

public class InventoryContainerComponent : MonoBehaviour
{
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
        for (int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(state);
        }
        ButtonTransform.localEulerAngles = Vector3.forward * (state ? 180 : 1);
        RectTransform parent = transform.parent.GetComponent<RectTransform>();
        layoutGroup.cellSize = Vector2.one * parent.rect.width * 0.475f;
    }
}